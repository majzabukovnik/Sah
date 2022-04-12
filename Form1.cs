using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private PictureBox[,] _chessBoard = new PictureBox[8, 8];//array za shranjevanje picture box
        Figura[,] figure = new Figura[8, 8];//array, ki shrani figure
        string poz = "";//začasna spremenljivka za hranjenje pozicij
        bool turn = true;//spremenljivka, ki spremlja, kdo ima potezo, kjer true pomeni bele figure in false črne figure
        int pozXB = 4;//spremenljicka, ki shranjuje pozicijo x od belega kralja
        int pozYB = 7;//spremenljicka, ki shranjuje pozicijo y od belega kralja
        int pozXC = 4;//spremenljicka, ki shranjuje pozicijo x od črnega kralja
        int pozYC = 0;//spremenljicka, ki shranjuje pozicijo y od črnega kralja

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            IzrisPolja();//metoda, ki izriše začetno polje
            for (int x = 0; x < 8; x++)//omogočanje spremljanja klikov na Picture box
            {
                for (int y = 0; y < 8; y++)
                {
                    _chessBoard[x, y].MouseClick += Form1_MouseClick;
                }
            }
            PostavitevFigur();//metoda, ki postavi figure v začetni položaj
        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)//metoda, ki ureja, kaj se zgodi, ko je nek picture box pritisnjen
        {
            PictureBox box = sender as PictureBox;
            int x = Convert.ToInt32(box.Name) / 10;//kordinata x, kamor se figura želi premakniti
            int y = Convert.ToInt32(box.Name) % 10;//kordinata y, kamor se figura želi premakniti
            if (poz.Length == 0 && figure[x, y] == null) poz = "";//če ni bil izbran še noben od picture boxov in na picture boxu ni figure
            else if (poz.Length == 2)//če je bila že izbrana začetna pozicija figure in konča pozicija
            {
                int x1 = Convert.ToInt32(poz) / 10;//kordinata x, od koder figura izvira
                int y1 = Convert.ToInt32(poz) % 10;//kordinata y, od koder figura izvira

                //ponovna nastavitev barve polja
                if ((x1 + y1) % 2 != 0) _chessBoard[x1, y1].BackColor = Color.DarkGray;
                else _chessBoard[x1, y1].BackColor = Color.White;

                if (Sah(figure) == true) ResitevSaha(x1, y1, x, y);

                else if (figure[x1, y1] != null && figure[x1, y1].LegalMove(x, y, turn, figure) == true && ResitevSaha(x1, y1, x, y, true) != true)
                {
                    if (figure[x1, y1].rosada == true) IzvrsitevRosade(x, y, x1, y1);//če je program pri preverjanju pravilnosti premika, ugotovil, da je rošada mogoča se izvede menjava dveh figur. Koda tega if stavka skrbi za izris in novo postavitev trdnajve, spodnja koda pa poskrbi za premik kralja
                    else PremikFigre(x, y, x1, y1);//navodila za premik figure iz pozicij [x1, y1] do [x, y]
                    if (figure[x, y] != null && figure[x, y].PawnPromotion() == true) PawnPromation(x, y);//če kmet pride do konca šahovnice, lahko prejme novo figuro
                    NajdiKralja(figure);//hranjenje nove pozicije kralja
                }
                poz = "";//brisanje vrednosti spremeljivke, ki hrani pozicijo premika
            }
            else
            {
                _chessBoard[x, y].BackColor = Color.FromArgb(100, 255, 240, 100);//označitev izbranega polja polja
                poz += x.ToString() + y.ToString();//hranjenje pozicij v spremeljivko
            }
        }
        bool ResitevSaha(int x1, int y1, int x, int y, bool s = false)//metoda za rešitev šaha in za zaznavanje, če bi premik lahko povzročil šah
        {
            Figura[,] figure1 = new Figura[8, 8];
            //Array.Copy(figure, figure1, figure.Length);
            figure1 = (Figura[,])figure.Clone();//kopija polja, ki hrani pozicje figur
            if (figure[x1, y1] != null && figure[x1, y1].LegalMove(x, y, turn, figure) == true)//preverja, če je premik dovoljen
            {
                //premik figur v polju, ki je kopija
                figure1[x, y] = figure1[x1, y1];
                figure1[x1, y1] = null;
                figure1[x, y].prviPremik = false;
                NajdiKralja(figure1);//spreminjanje shranjene pozicije kralja
            }
            if (Sah(figure1) == false && s == false) PremikFigre(x, y, x1, y1); //preverimo, če smo se s premikom rešili šaha in s, ki je false nam pove, da je šah ze povzročen
            else if (s == true && Sah(figure1) == true) return true;//če je šah true, s, ki je true, pa pove, da samo preverjamo če premik povzroči šah - potem tak premik ne dovilimo
            NajdiKralja(figure);//spreminjanje shranjenjene pozicje kralja
            return false;//ne uporabimo
        }
        void NajdiKralja(Figura[,] figureList)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    if (figureList[x, y] != null && figureList[x, y].barva == true && figureList[x, y].name == "kralj") { pozXB = x; pozYB = y; }
                    else if (figureList[x, y] != null && figureList[x, y].barva == false && figureList[x, y].name == "kralj") { pozXC = x; pozYC = y; }
                }
            }
        }
        bool Sah(Figura[,] figureList)//preverjanje za šahom
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (figureList[x, y] != null)
                    {
                        if (figureList[x, y].barva == true)
                        {
                            if (figureList[x, y].LegalMove(pozXC, pozYC, true, figureList) == true) return true;//zazna šah nad črnim kraljem
                        }
                        else
                        {
                            if (figureList[x, y].LegalMove(pozXB, pozYB, false, figureList) == true) return true;//zazna šah nad belim kraljem
                        }
                    }
                }
            }
            return false;
        }
        void IzvrsitevRosade(int x, int y, int x1, int y1)//metoda za rošado
        {
            if (x == 7)//spodaj so navodila za premik trdnjave
            {
                figure[x - 2, y] = figure[x, y];
                figure[x, y] = null;
                _chessBoard[x - 2, y].BackgroundImage = _chessBoard[x, y].BackgroundImage;
                _chessBoard[x, y].BackgroundImage = null;
                figure[x - 2, y].pozX = x - 2;
                figure[x - 2, y].pozY = y;
                x--;
            }
            else if (x == 0)//spodaj so navodila za premik trdnjave
            {
                figure[x + 3, y] = figure[x, y];
                figure[x, y] = null;
                _chessBoard[x + 3, y].BackgroundImage = _chessBoard[x, y].BackgroundImage;
                _chessBoard[x, y].BackgroundImage = null;
                figure[x + 3, y].pozX = x + 3;
                figure[x + 3, y].pozY = y;
                x += 2;
            }
            figure[x1, y1].rosada = false;

            PremikFigre(x, y, x1, y1);
        }
        void PremikFigre(int x, int y, int x1, int y1)//metoda, za premik figure
        {
            _chessBoard[x, y].BackgroundImage = _chessBoard[x1, y1].BackgroundImage;
            figure[x, y] = figure[x1, y1];
            _chessBoard[x1, y1].BackgroundImage = null;
            figure[x1, y1] = null;
            figure[x, y].pozX = x;
            figure[x, y].pozY = y;
            figure[x, y].prviPremik = false;
            turn = !turn;
        }
        void PawnPromation(int x, int y)//metoda za povišanje kmeta
        {
            Form3 form2 = new Form3();//klic form2
            string ime = form2.Izbira();//dobim podatek o željeni figuri
            bool barva = figure[x, y].barva;//shrfanjevanje bare kmeta
            string directory = Environment.CurrentDirectory;//trenutna pozicija v pomnilniku
            if (figure[x, y].barva == true) directory += @"\Figure\b_" + @ime + @".png";//prikaz slike za bele figure
            else directory += @"\Figure\c_" + @ime + @".png";//prikaz slike za črne figure
            figure[x, y].Nastavitev(ime, x, y, barva);//nastavitev nove figure
            _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory);//prikaz slike figure
        }

        void IzrisPolja()//metoda, ki izriše začetno polje
        {
            const int tileSize = 80;
            const int gridSize = 8;
            this.Width = 655;
            this.Height = 678;
            this.Text = "Šah";

            _chessBoard = new PictureBox[gridSize, gridSize];//inacializacija

            for (int n = 0; n < 8; n++)//vrstice
            {
                for (int m = 0; m < 8; m++)//stolpci
                {
                    var newPanel = new PictureBox//ustvarjen nov PictureBox
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * n, tileSize * m)
                    };

                    Controls.Add(newPanel);//prikaz elementa

                    _chessBoard[n, m] = newPanel;//shranjevanje Panel v array
                    newPanel.Name = $"{n}{m}";//nastavitev imena picture box; ime je sestavljeno iz x in y kordinate, glede na lokacjo picture boxa

                    //določanje barve polja
                    if ((n + m) % 2 != 0) newPanel.BackColor = Color.DarkGray;
                    else newPanel.BackColor = Color.White;
                }
            }
        }

        void PostavitevFigur()//metoda, ki postavi figure v začetni položaj
        {
            int x;
            int y = 0;
            string directory = Environment.CurrentDirectory;

            //postavitev črnih figur in ustvariti primer razreda
            for (x = 0; x < 8; x += 7) //trdnjava
            {
                figure[x, y] = new Figura(); //ustvarjena nova figura na istem mestu v array, kot se ta figura nahaja v arrayu s picture boxi
                figure[x, y].Nastavitev("trdnjava", x, y, false); //nastavitev parametrov figure s pomočjo metode Nastavitev()
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_trdnjava.png"); //nastavitev slike od picture boxa
            }
            for (x = 1; x < 8; x += 5)//konj
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("konj", x, y, false);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_konj.png");
            }
            for (x = 2; x < 8; x += 3)//tekač
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("tekac", x, y, false);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_tekac.png");
            }

            figure[4, y] = new Figura();//kralj
            figure[4, y].Nastavitev("kralj", 4, y, false);
            _chessBoard[4, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_kralj.png");

            figure[3, y] = new Figura();//kraljica
            figure[3, y].Nastavitev("kraljica", 3, y, false);
            _chessBoard[3, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_kraljica.png");

            y++;

            for (x = 0; x < 8; x++)//kmet
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("kmet", x, y, false);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\c_kmet.png");
            }
            y += 6;

            //postavitev belih figur
            for (x = 0; x < 8; x += 7) //trdnjava
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("trdnjava", x, y, true);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_trdnjava.png");
            }
            for (x = 1; x < 8; x += 5) //konj
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("konj", x, y, true);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_konj.png");
            }
            for (x = 2; x < 8; x += 3)//tekač
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("tekac", x, y, true);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_tekac.png");
            }

            figure[4, y] = new Figura();//kralj
            figure[4, y].Nastavitev("kralj", 4, y, true);
            _chessBoard[4, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_kralj.png");

            figure[3, y] = new Figura();//kraljica
            figure[3, y].Nastavitev("kraljica", 3, y, true);
            _chessBoard[3, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_kraljica.png");

            y--;

            for (x = 0; x < 8; x++) //kmet
            {
                figure[x, y] = new Figura();
                figure[x, y].Nastavitev("kmet", x, y, true);
                _chessBoard[x, y].BackgroundImage = Image.FromFile(@directory + @"\Figure\b_kmet.png");
            }
        }
    }
}

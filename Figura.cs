using System;


namespace WindowsFormsApp1
{
    public class Figura
    {
        public string name = "";
        public int pozX = 0;
        public int pozY = 0;
        public bool barva = true;//true pomeni bela figura, med tem ko false pomeni črna figura
        public bool prviPremik = true;
        public bool rosada = false;

        public void Nastavitev(string name, int x, int y, bool barva)
        {
            this.name = name;
            this.pozX = x;
            this.pozY = y;
            this.barva = barva;
        }
        public bool LegalMove(int x, int y, bool turn, Figura[,] figure)
        {
            if (pozX == x && pozY == y) return false;//onemogočanje premika te figure na mesto, kjer že stoji ta figura
            if (turn != this.barva) return false;//preverjanje, če ima igrelec prestavlja svojo oz. nasporotnikovo figuro

            switch (this.name)
            {
                case "kmet"://manjkajoč En Passant 
                    return LegalKmet(x, y, figure);

                case "kraljica":
                    if (LegalTekac(x, y, figure) == true || LegalTrdnjava(x, y, figure) == true) return true;
                    else return false;

                case "kralj":
                    return LegalKralj(x, y, figure);

                case "tekac":
                    return LegalTekac(x, y, figure);

                case "konj":
                    return LegalKonj(x, y, figure);

                case "trdnjava":
                    return LegalTrdnjava(x, y, figure);

                default:
                    return false;
            }
        }
        public bool Rosada(int x, int y, Figura[,] figure)
        {
            if ((this.prviPremik == false || (figure[x, y] != null && figure[x, y].prviPremik == false)) || y != pozY) return false;//preverjanje, če je to prvi premik kralja in trdnjave
            else if (x == 0)//rošada na stran kraljice
            {
                while (x < pozX - 1)
                {
                    x++;
                    if (figure[x, y] != null) return false;
                }
                rosada = true;
                return true;
            }
            else if (x == 7)//rošada na kraljevo stran
            {
                while (x > pozX + 1)
                {
                    x--;
                    if (figure[x, y] != null) return false;
                }
                rosada = true;
                return true;
            }
            return false;
        }
        public bool PawnPromotion()
        {
            if ((pozY == 7 || pozY == 0) && this.name == "kmet") return true;
            else return false;
        }
        
        bool LegalKmet(int x, int y, Figura[,] figure)
        {
            if (barva == true)
            {
                if ((pozY - 2 == y && prviPremik == true && figure[pozX, pozY - 1] == null) || pozY - 1 == y)//preverjanje ali je dovoljen premik za dve mesti oz. le za eno
                {
                    if (pozX == x && figure[x, y] == null) return true;//preverjanje ali je premik naprej dovoljen
                    else if ((pozX - 1 == x || pozX + 1 == x) && figure[x, y] != null && figure[x, y].barva == false) return true;//preverjanje, če je dovoljeno premakniti kmeta diagonalno
                }
            }
            else
            {
                if ((pozY + 2 == y && prviPremik == true && figure[pozX, pozY + 1] == null) || pozY + 1 == y)//preverjanje ali je dovoljen premik za dve mesti oz. le za eno
                {
                    if (pozX == x && figure[x, y] == null) return true;//preverjanje ali je premik naprej dovoljen
                    else if ((pozX - 1 == x || pozX + 1 == x) && figure[x, y] != null && figure[x, y].barva == true) return true;//preverjanje, če je dovoljeno premakniti kmeta diagonalno
                }
            }
            return false;
        }

        bool LegalTrdnjava(int x, int y, Figura[,] figure)
        {
            if (pozX == x)
            {
                if (pozY - y > 0)
                {
                    if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;//preverjanje, da slučajo igralec ne želi zbiti svoje figure
                    for (int i = pozY - 1; i > y; i--)
                    {
                        if (figure[x, i] != null) return false;//preverjanje, da so vsa polja do cilnega polja prazna
                    }
                }
                if (pozY - y < 0)
                {
                    if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;//preverjanje, da slučajo igralec ne želi zbiti svoje figure
                    for (int i = pozY + 1; i < y; i++)
                    {
                        if (figure[x, i] != null) return false;//preverjanje, da so vsa polja do cilnega polja prazna
                    }
                }
            }
            else if (pozY == y)
            {
                if (pozX - x > 0)
                {
                    if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;//preverjanje, da slučajo igralec ne želi zbiti svoje figure
                    for (int i = pozX - 1; i > x; i--)
                    {
                        if (figure[i, y] != null) return false;//preverjanje, da so vsa polja do cilnega polja prazna
                    }
                }
                if (pozX - x < 0)
                {
                    if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;//preverjanje, da slučajo igralec ne želi zbiti svoje figure
                    for (int i = pozX + 1; i < x; i++)
                    {
                        if (figure[i, y] != null) return false;//preverjanje, da so vsa polja do cilnega polja prazna
                    }
                }
            }
            else return false;//če se x in y kordinati ciljnega polja hkrati razlikujeta od trenutne pozicje to razveljavi potezo
            return true;
        }

        bool LegalKonj(int x, int y, Figura[,] figure)
        {
            if (((pozX + 2 == x && (pozY + 1 == y || pozY - 1 == y)) ||
               (pozX - 2 == x && (pozY + 1 == y || pozY - 1 == y)) ||
               (pozY + 2 == y && (pozX + 1 == x || pozX - 1 == x)) ||
               (pozY - 2 == y && (pozX + 1 == x || pozX - 1 == x))) &&
               (figure[x, y] == null || figure[x, y].barva != this.barva)) return true;//preverjanje vseh možnih L potez, ki jih konj lahko izvede ter preverjanje, da na cilju ni naše figure
            return false;
        }

        bool LegalKralj(int x, int y, Figura[,] figure)
        {
            if (Math.Abs(pozY - y) > 1 && Math.Abs(pozX - x) > 1) return false;
            if (Rosada(x, y, figure) == true) return true;
            else if (((pozY + 1 == y && x == pozX) ||
                        (pozY - 1 == y && x == pozX) ||
                        (pozX + 1 == x && pozY == y) ||
                        (pozX - 1 == x && pozY == y) ||
                        (pozX + 1 == x && pozY + 1 == y) ||
                        (pozX - 1 == x && pozY + 1 == y) ||
                        (pozX + 1 == x && pozY - 1 == y) ||
                        (pozX - 1 == x && pozY - 1 == y)) &&
                        (figure[x, y] == null || figure[x, y].barva != this.barva))//preverjanje vseh možnih strani, kamor se lahko kralj premakne
            {
                return true;
            }
            return false;
        }

        bool LegalTekac(int x, int y, Figura[,] figure)
        {
            if (Math.Abs(pozY - y) != Math.Abs(pozX - x)) return false;
            if (pozX - 2 == x && pozY - 4 == y) return false;
            if (pozX > x && pozY > y)//diagonala levo gor
            {
                if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;
                if ((x + y) % 2 != (pozX + pozY) % 2) return false;
                while (x + 1 < pozX)
                {
                    x++;
                    y++;
                    if (figure[x, y] != null) return false;
                }
                return true;
            }
            else if (pozX > x && pozY < y)//diagonala levo dol
            {
                if (pozX - 2 == x && pozY + 4 == y) return false;
                if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;
                if ((x + y) % 2 != (pozX + pozY) % 2) return false;
                while (x < pozX - 1)
                {
                    x++;
                    y--;
                    if (figure[x, y] != null) return false;
                }
                return true;
            }
            else if (pozX < x && pozY < y)//diagonala desno dol
            {
                if (pozX + 2 == x && pozY + 4 == y) return false;
                if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;
                if ((x + y) % 2 != (pozX + pozY) % 2) return false;
                while (x - 1 > pozX)
                {
                    x--;
                    y--;
                    if (figure[x, y] != null) return false;
                }
                return true;
            }
            else if (pozX < x && pozY > y)//diagonala desno gor
            {
                if (pozX + 2 == x && pozY - 4 == y) return false;
                if (figure[x, y] != null && figure[x, y].barva == this.barva) return false;
                if ((x + y) % 2 != (pozX + pozY) % 2) return false;
                while (x > pozX + 1)
                {
                    x--;
                    y++;
                    if (figure[x, y] != null) return false;
                }
                return true;
            }
            return false;
        }
    }
}

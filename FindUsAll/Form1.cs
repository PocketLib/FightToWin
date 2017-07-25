using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Threading;



namespace FindUsAll
{
    public struct buttonSet // tworzy strukturę, która będzie miała dwa pola - set i character
    {
        public bool set;
        public char character;
    }

    public partial class Form1 : Form // klasa główna programu (w tym przypadku jedyna)
    {
        const int clicksToWin = 100; // ile razy trzeba kliknąć żeby wygrać, jest jako stała bo dzięki temu można tu zmienić i zmieni się wszędzie, nie trzeba szukać po kodzie
        buttonSet[] buttons = new buttonSet[4]; // tablica zawierająca informacje o przyciskach przypisanych do gracza
        int[] clicksLeft = new int[2]; // wyniki dla obu graczy
        char waiting; // przycisk oczekujący na przypisane do danej akcji (dodawanie/odejmowanie punktów)
        bool isWaiting = false; // czy jakiś przycisk oczekuje na przypisanie (patrz linijka wyżej)
        bool ready = false; // czy wszystkie przyciski są ustawione (można grać)
        bool finished = false; // czy ktoś już wygrał

        public Form1() // konstruktor - funkcja wywoływana przy tworzeniu klasy
        {
            InitializeComponent(); // ustaw całe UI
            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].set = false; // ustawiamy wszystkie przyciski jako wymagające ustawienia
            }
            for (int i = 0; i < clicksLeft.Length; i++)
            {
                clicksLeft[i] = clicksToWin; // obu graczom ustawiamy wymaganą ilość kliknięć do wygrania
            }
            SetUp();
        }

        private void EventKeyPress(object sender, KeyPressEventArgs e) // jest przypisane w Design do okna programu jako do wykonania gdy zostanie wciśnięty przycisk na klawiaturze
        {
            if (!ready) // jeśli jeszcze nie ukończono ustawiania klawiszy (albo jakiś jest do zmiany)
                setWaitingKey(e.KeyChar);
            else if (!finished) // jeśli poprzednie jest fałeszem (jeśli wszystko ustawione) i nikt nie wygrał
                checkScores(e.KeyChar);
        }

        private void setWaitingKey(char key)
        {
            waiting = key; // ustaw wciśnięty przycisk jako oczekujący na przypisanie do akcji
            isWaiting = true; // zaznacz że przycisk oczekuje przypisania
            SetUp();
        }

        private void checkScores(char key)
        {
            int index = getOperationIndex(key); //na podstawie wciśniętego przycisku ustal do jakiej akcji jest on przypisany
            if (index != -1) // jeśli jest do jakiejś przypisany to...
            { // index to od 0 do 3, 0,1 to gracz1, 2,3 to gracz 2 (odpowiednio dodawanie i odejmowanie punktów)
                int operation = index % 2; // akcja to reszta z dzielenia przez 2 (jeśli 0 to dodawania, 1 odejmowanie punktów)
                int player = index / 2; // gracz którego akcja dotyczy to wynik dzielenia całkowitego indexu przez 2 (0 - player1, 1 - player2)
                ChangeScore(operation, player);
                UpdateScores();
            }
        }

        private int getOperationIndex(char pressed)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].character == pressed)
                {
                    return i; // sprawdź czy wśród wszystkich klawiszy przypisanych do akcji jest ten, który został wciśnięty, jeśli tak to zwróć indeks tej akcji
                }
            }
            return -1; // jeśli nie to zwróć -1 (umownie - nie ma takiego indeksu)
        }

        private void ChangeScore(int operation, int player)
        {
            if (operation == 0) // jeśli operacja to dodawanie
                clicksLeft[player]++;
            else
                clicksLeft[player]--;
            CheckIfTheEnd(); // sprawdź czy ktoś wygrał
        }

        private void CheckIfTheEnd()
        {
            for (int i = 0; i < clicksLeft.Length; i++)
            {
                if (clicksLeft[i] <= 0) // dla każdego gracza sprawdź czy komuś zostało maksymalnie 0 kliknięć do wygrania (w razie gdybyśmy jakieś przegapili)
                    GameOver(i); // jak tak to game over
            }
        }

        private void UpdateScores() // zaktualizuj wyniki wyświetlane w UI
        {
            p1score.Text = clicksLeft[0].ToString();
            p2score.Text = clicksLeft[1].ToString();
        }

        private void description_Click(object sender, EventArgs e) // wywoływane jak ktoś kliknie w etykietę opisującą dany przycisk
        {
            Label label = (Label)sender; // rzutowanie typów - typ Label dziedziczy po typie object więc można powiedzieć potraktuj object jako label
            int buttonIndex = getButtonIndex(label);
            setButtons(buttonIndex, "");
            SetUp();
        }

        private void setButtons(int buttonIndex, string character)
        { // wykorzystywana struktura: int x = (A)?B:C; jeśli A jest prawdą to x = B, w przeciwnym wypadku x = C

            //TODO: sprawdzanie czy dany przycisk nie jest już wykorzystywany (ale najpierw ogarnij to co tu jest, wtedy to szybko dodam) (musiałbym się pozbyć struktury (A)?B:C żeby to dodać)

            buttons[buttonIndex].set = (character != "")?true:false; // jeśli character to nie pusty string to ustaw pole set w tablicy buttons na indeksie buttonIndex na true, w przeciwnym wypadku na false
            if (character == "")
                ready = false; // jeśli character jest pusty to znaczy że chcemy zmienić przcisk - nie można jeszcze grać
            if (character != "")
            {
                buttons[buttonIndex].character = character[0]; // w przeciwnym wypadku do danego przycisku przypisz kod klawisza wciśnięty przez użytkownika
            }
            Label buttonLabel = getButtonLabel(buttonIndex);
            buttonLabel.Text = character.ToString(); // zaktualizuj też przypisywany przycisk w UI
        }

        private int getButtonIndex(Label sender)
        {
            string name = sender.Name; // nazwa wciśniętej etykiety (ustalone na np desc0p), patrz design view
            int playerID = (int)char.GetNumericValue(name[4]); // id gracza znajduje się w nazwie jako piąty znak (tabloce zaczynają się od 0), w przykładzie jest to 0
            bool adding = (name[5] == 'p') ? true : false; // jeśli na 6. pozycji jest p to znaczy że akcja to dodawanie, w przeciwnym wypadku odejmowanie
            return (adding) ? playerID * 2 : playerID * 2 + 1; // zwróc index przycisku - dwukrotność id gracza dla dodawania, dwukrotność ID gracza + 1 dla odejmowania
        } // stosowany system indexów: 0 - gracz1+, 1 - gracz1-, 2 - gracz2+, 3 - gracz2-

        private Label getButtonLabel(int index)
        {
            switch (index) // na podstawie indeksu ustal w jakiej etykiecie musimy w UI zmienić napis na wybrany przez użytkownika klawisz
            {
                case 0:
                    return selectedButton0;
                case 1:
                    return selectedButton1;
                case 2:
                    return selectedButton2;
                case 3:
                    return selectedButton3;
                default:
                    return null;
            }
        }

        private void GameOver(int won)
        {
            finished = true; // ustaw zmienną na fakt że rozgrywka skończona (unikniemy np. dalszego naliczania puktów)
            string message = String.Format("Player {0} won!", won + 1); // ustaw tekst wiadomości - zamiast {0} będzie pierwszy argument po stringu, tutaj (won+1), odpowiednio {1} to drugi, {2} to trzeci itd, patrz funkcja setLabels()
            string caption = "Game Over!"; // tytuł powiadomienia
            MessageBoxButtons buttons = MessageBoxButtons.OK; // powiadomienie ma mieć przycisk OK
            DialogResult result; // w tym będzie informacja jaki przycisk został wciśnięty

            result = MessageBox.Show(message, caption, buttons); // wyświetl powiadomienie i przypisz wciśnięty przycisk do result

            if (result == DialogResult.OK)
                this.Close(); // jak ktoś wcisnął OK to zakończ program
        }

        private void SetUp()
        {
            if (isWaiting)
                tryToSetButton(); // jeśli jakiś przycisk oczekuje na przypisanie to go przypisz
            SetLabels(); // ustaw etykietę status w UI
        }

        private void tryToSetButton()
        {
            int index = getIndexToSet(); // pobierz indeks do ustawienia przycisku

            if (index != -1) // jeśli taki przycisk istnieje to...
            {
                setButtons(index, waiting.ToString()); //ustaw odpowiednio przycisk 
                getButtonLabel(index).Text = waiting.ToString().ToUpper(); // i etykietę w UI
                isWaiting = false; // zaznacz że aktualnie oczekujący na zarejestrowanie przycisk już jest zarejestrowany
            }
        }

        private void SetLabels()
        {
            int index = getIndexToSet(); // pobierz indeks do ustawienia przycisku

            if (index == -1) // jeśli taki przycisk nie istnieje to...
            {
                ready = true; // można grać (przyciski ustawione)
                status.Text = "GRAJ!"; // ustal odpowiednio tekst etykiety status
            }
            else
            {
                int player = index / 2; // indeksy patrz w CheckScores()
                int operation = index % 2;
                status.Text = String.Format("Press {0} button for player {1}", (operation == 0) ? "add" : "subtract", player + 1); // wyjaśnienie składni w gameOver()
            }
        }

        private int getIndexToSet()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].set == false) // jeśli jakiś przycisk ma ustawione że nie jest jeszcze ustawiony to zwróć jego index
                    return i;
            }
            return -1; // w przeciwnym wypadku -1
        }
    }
}

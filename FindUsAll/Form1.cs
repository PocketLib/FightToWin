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
    public struct buttonSet 
    {
        public bool set;
        public char character;
    }

    public partial class Form1 : Form 
    {
        const int clicksToWin = 100; 
        buttonSet[] buttons = new buttonSet[4]; 
        int[] clicksLeft = new int[2]; 
        char waiting; 
        bool isWaiting = false; 
        bool ready = false; 
        bool finished = false; 

        public Form1() 
        {
            InitializeComponent();  
            for (int i = 0; i < buttons.Length; i++) 
            {
                buttons[i].set = false; 
            }
            for (int i = 0; i < clicksLeft.Length; i++)
            {
                clicksLeft[i] = clicksToWin; 
            }
            SetUp();
        }

        private void EventKeyPress(object sender, KeyPressEventArgs e) 
        {
            if (!ready) 
                setWaitingKey(e.KeyChar);
            else if (!finished) 
                checkScores(e.KeyChar);
        }

        private void setWaitingKey(char key)
        {
            waiting = key; 
            isWaiting = true; 
            SetUp();
        }

        private void checkScores(char key)
        {
            int index = getOperationIndex(key); 
            if (index != -1) 
            { 
                int operation = index % 2; 
                int player = index / 2; 
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
                    return i; 
                }
            }
            return -1; 
        }

        private void ChangeScore(int operation, int player)
        {
            if (operation == 0) 
                clicksLeft[player]++;
            else
                clicksLeft[player]--;
            CheckIfTheEnd(); 
        }

        private void CheckIfTheEnd()
        {
            for (int i = 0; i < clicksLeft.Length; i++)
            {
                if (clicksLeft[i] <= 0) 
                    GameOver(i); 
            }
        }

        private void UpdateScores() 
        {
            p1score.Text = clicksLeft[0].ToString();
            p2score.Text = clicksLeft[1].ToString();
        }

        private void description_Click(object sender, EventArgs e) 
        {
            Label label = (Label)sender; 
            int buttonIndex = getButtonIndex(label);
            setButtons(buttonIndex, "");
            SetUp();
        }

        private void setButtons(int buttonIndex, string character)
        { 
            if (character != "")
                ChangeButton(buttonIndex, character);
            else
            {
                buttons[buttonIndex].set = false;
                ready = false;
                Label buttonLabel = getButtonLabel(buttonIndex);
                buttonLabel.Text = character.ToString();
            }
        }

        private void ChangeButton(int buttonIndex, string character)
        {
            if (!CheckIfKeyBinded(buttonIndex, character))
            {
                buttons[buttonIndex].set = true;
                buttons[buttonIndex].character = character[0];
                Label buttonLabel = getButtonLabel(buttonIndex);
                buttonLabel.Text = character.ToString().ToUpper();
            }
        }

        private bool CheckIfKeyBinded(int buttonIndex, string character)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i != buttonIndex && buttons[i].character == character[0] && buttons[i].set == true)
                    return true;
            }
            return false;
        }

        private int getButtonIndex(Label sender)
        {
            string name = sender.Name; 
            int playerID = (int)char.GetNumericValue(name[4]); 
            bool adding = (name[5] == 'p') ? true : false; 
            return (adding) ? playerID * 2 : playerID * 2 + 1; 
        } 

        private Label getButtonLabel(int index)
        {
            switch (index) 
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
            finished = true; 
            string message = String.Format("Player {0} won!", won + 1); 
            string caption = "Game Over!"; 
            MessageBoxButtons buttons = MessageBoxButtons.OK; 
            DialogResult result; 

            result = MessageBox.Show(message, caption, buttons); 

            if (result == DialogResult.OK)
                this.Close(); 
        }

        private void SetUp()
        {
            if (isWaiting)
                tryToSetButton(); 
            SetLabels(); 
        }

        private void tryToSetButton()
        {
            int index = getIndexToSet(); 

            if (index != -1) 
            {
                setButtons(index, waiting.ToString()); 
                isWaiting = false; 
            }
        }

        private void SetLabels()
        {
            int index = getIndexToSet(); 

            if (index == -1) 
            {
                ready = true; 
                status.Text = "GRAJ!"; 
            }
            else
            {
                int player = index / 2; 
                int operation = index % 2;
                status.Text = String.Format("Press {0} button for player {1}", (operation == 0) ? "add" : "subtract", player + 1); 
            }
        }

        private int getIndexToSet()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].set == false) 
                    return i;
            }
            return -1; 
        }
    }
}

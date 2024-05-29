using System.Drawing.Text;

namespace Wordle
{
    public partial class WordleForm : Form
    {
        private const string WordsTextFile = @"wordsForWordle.txt";
        private const int RowLength = 5;
        private const string PlayAgainMessage = "Play again?";
        private int previousRow = 0;
        private int hintsCount = 0;
        private string currentWord = string.Empty;
        private List<TextBox> currentBoxes = new List<TextBox>();
        public WordleForm()
        {
            InitializeComponent();
            StartNewGame();
            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                tb.MouseClick += this.FocusTextBox;
                tb.KeyDown += this.MoveCursor;
            }
        }
        private void FocusTextBox(object sender, MouseEventArgs e)
        {

            if (sender is TextBox textBox)
            {
                textBox.Focus();
            }
        }
        private bool ShouldGoToLeftTextBox(Keys pressedKey, int currentTextBoxIndex) => pressedKey == Keys.Left && !IsFirstTextBox(currentTextBoxIndex);
        private bool IsFirstTextBox(int currentTextBoxIndex) => (currentTextBoxIndex + 4) % RowLength == 0;
        private bool ShouldGoToRichtTextBox(Keys pressedKey, int currentTextBoxIndex) => (pressedKey == Keys.Right || IsAlphabetKeyPressed(pressedKey.ToString())) && !IsLastTextBox(currentTextBoxIndex);
        private bool IsLastTextBox(int currentTextBoxIndex) => currentTextBoxIndex % RowLength == 0;
        private bool IsAlphabetKeyPressed(string pressedKeystring) => pressedKeystring.Count() == 1 && char.IsLetter(pressedKeystring[0]);
        private void MoveCursor(object sender, KeyEventArgs e)
        {
            var pressedKey = e.KeyCode;
            var senderTextBox = sender as TextBox;
            var currentTextBoxIndex = int.Parse(senderTextBox.Name.Replace("textBox", ""));
            if (ShouldGoToLeftTextBox(pressedKey, currentTextBoxIndex))
            {
                currentTextBoxIndex--;
            }
            else if (ShouldGoToLeftTextBox(pressedKey, currentTextBoxIndex))
            {
                currentTextBoxIndex++;
            }

            var textBox = GetTextBox(currentTextBoxIndex);
            textBox.Focus();
        }

        private TextBox GetTextBox(int index)
        {
            string textBoxName = string.Format("textBox{0]", index);
            return this.Controls[textBoxName] as TextBox;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void StartNewGame()
        {
            var wordList = GetAllWords();

            var random = new Random();

            currentWord = wordList[random.Next(wordList.Count)];

            btnSubmit.Enabled = true;
            btnHint.Enabled = true;
        }
        private List<string> GetAllWords()
        {
            var allWords = new List<string>();
            using (StreamReader reader = new StreamReader(WordsTextFile))
            {
                while (!reader.EndOfStream)
                {
                    var nextLine = reader.ReadLine();
                    allWords.Add(nextLine);
                }
            }
            return allWords;
        }
        private void Submit(object sender, EventArgs e)
        {
            var userWord = GetInput();
            if (!IsInputValid(userWord))
            {
                DisplayInvaLidWordMessage();
                return;
            }

            ColorBoxes();

            if (IsWordGuessed(userWord))
            {
                FinalizeWinGame();
                return;
            }
            if (IsCurrentRowLast())
            {
                FinalizeLostGame();
                return;
            }
            ModifyTextBoxesAvailability(false);
            previousRow++;
            ModifyTextBoxesAvailability(true);
        }
        private string GetInput()
        {
            this.currentBoxes = new List<TextBox>();
            string tempString = string.Empty;

            int firstTextBoxIndexOnRow = GetFirstTextBoxIndexOnRow();

            for (int i = 0; i < RowLength; i++)
            {
                var textBox = GetTextBox(firstTextBoxIndexOnRow + i);
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    return textBox.Text;
                }
                tempString += textBox.Text[0];
                this.currentBoxes.Add(textBox);
            }
            return tempString;
        }
        private int GetFirstTextBoxIndexOnRow() => this.previousRow * RowLength + 1;
        private bool IsInputValid(string input)
        {
            if (input.All(char.IsLetter) && input.Length == RowLength)
            {
                return true;
            }
            return false;
        }
        private void DisplayInvaLidWordMessage()
        {
            MessageBox.Show("Plase enter a valid five-letter word.");
        }
        private void ColorBoxes()
        {
            for (int i = 0; i < this.currentBoxes.Count(); i++)
            {
                var textBox = this.currentBoxes[i];
                var currentTextBoxChar = textBox.Text.ToLower().FirstOrDefault();

                if (!WordContainsChar(currentTextBoxChar))
                {
                    textBox.BackColor = Color.Gray;
                }
                else if (!IsCharOnCorrectIndex(i, currentTextBoxChar))
                {
                    textBox.BackColor = Color.Yellow;
                }
                else
                {
                    textBox.BackColor = Color.LightGreen;
                }
            }
        }
        private bool WordContainsChar(char ch) => this.currentWord.Contains(ch, StringComparison.OrdinalIgnoreCase);
        private bool IsCharOnCorrectIndex(int index, char ch) => this.currentWord[index] == ch;

        private bool IsWordGuessed(string attempt)
        {
            if (this.currentWord.Equals(attempt, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        private void FinalizeWinGame()
        {
            MessageBox.Show("Congratulations, you win!");

            this.btnSubmit.Enabled = false;
            this.btnHint.Enabled = false;

            this.btnReset.Text = PlayAgainMessage;

            ModifyTextBoxesAvailability(false);
        }
        private void ModifyTextBoxesAvailability(bool shouldBeEnabled)
        {
            var firstTextBoxIndexOnRow = GetFirstTextBoxIndexOnRow();
            for (int i = 0; i < RowLength; i++)
            {
                var textBox = GetTextBox(firstTextBoxIndexOnRow + i);

                if (shouldBeEnabled)
                {
                    textBox.Enabled = true;
                    if (i == 0)
                    {
                        textBox.Focus();
                    }
                }
                else
                {
                    textBox.ReadOnly = true;
                    textBox.TabStop = false;
                }
            }
        }
        private bool IsCurrentRowLast()
        {
            var columnsCout = 6;
            return this.previousRow == columnsCout - 1;
        }
        private void FinalizeLostGame()
        {
            MessageBox.Show($"Sorry you didn't win this time!" + $" The correct word was: {this.currentWord}");
            btnSubmit.Enabled = false;
            btnHint.Enabled = false;
            btnReset.Text = PlayAgainMessage;
        }
        private void btnReset_Click(object sender, EventArgs e)
        {

        }
        private void GameRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void GiveHist(object sender, EventArgs e)
        {
            var unavailablePositions = GeetUnavaillblePositions();
            if (unavailablePositions.Count == RowLength)
            {
                ShowInvalidUseOfHintMassage();
                return;
            }
            RevealRandomWordLetter(unavailablePositions);
        }
        private List<int> GeetUnavaillblePositions()
        {
            var firstIndexOnRow = GetFirstTextBoxIndexOnRow();
            var positions = new List<int>();
            for (int i = 0; i < RowLength; i++)
            {
                var textBoxIndex = firstIndexOnRow + i;
                var textBox = GetTextBox(textBoxIndex);
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    positions.Add(textBoxIndex);
                }
            }
            return positions;
        }
        private void ShowInvalidUseOfHintMassage()
        {
            MessageBox.Show("Free up a space for a hint.");
            this.btnSubmit.Focus();
            this.hintsCount -= 1;
        }
        private void RevealRandomWordLetter(List<int> unavailablePositions)
        {
            var random = new Random();
            while (true)
            {
                var randomIndex = random.Next(1, RowLength + 1);
                var randomTexBoxIndex = this.previousRow * RowLength + randomIndex;              
                var textBox = GetTextBox(randomTexBoxIndex);
                if (textBox.Text != String.Empty)
                {
                    continue;
                }
                var hintLetter = this.currentWord[randomIndex - 1].ToString();
                textBox.Text = hintLetter;
                unavailablePositions.Add(randomTexBoxIndex);

                break;
            }
        }

        private void HintCounterMouseClick(object sender, MouseEventArgs e)
        {
            this.hintsCount++;
            if (this.hintsCount >= 3)
            {
                this.btnHint.Enabled = false;
            }
        }
    }
}
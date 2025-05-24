using System.Diagnostics;
using EightPuzzleLibrary;

namespace _8_PuzzleSolver
{
    public partial class Form : System.Windows.Forms.Form
    {
        string? Algorithm;
        int[,] Puzzle;
        Button[,] Buttons;

        SortedSet<int> Pieces;
        List<List<Node>> Solutions;
        List<Node>? Current;

        public Form()
        {
            InitializeComponent();
            Pieces = new SortedSet<int>();
            Solutions = new List<List<Node>>();
            Puzzle = new int[3, 3];

            for (int i = 1; i < 9; i++)
                Pieces.Add(i);
            Buttons = new Button[3, 3]
            {
                { button1, button2, button3 },
                { button4, button5, button6 },
                { button7, button8, button9 }
            };
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            buttonSolve.Enabled = false;
            if (Pieces.Count > 0)
            {
                MessageBox.Show("Заповніть пазл до кінця!", "Заповніть пазл", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (!IsPuzzleSolvable())
            {
                MessageBox.Show("Поточний пазл є нерозв'язним", "Нерозв'язність", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (listBoxSolutions.Items.Count > 15)
            {
                MessageBox.Show("Забагато збережених розв'язків. Видаліть один з них", "Переповнення", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Stopwatch stopwatch = new Stopwatch();            
            stopwatch.Start();                 
            switch (Algorithm)
            {
                case "A*":
                    Current = A_Star_Algorithm.Solve(Puzzle);                   
                    break;
                case "RBFS":
                    Current = RBFS_Algorithm.Solve(Puzzle);                   
                    break;
                default:
                    MessageBox.Show("Оберіть алгоритм пошуку!", "Оберіть алгоритм", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
            }      
            stopwatch.Stop();           
            PrintSolution(Current, textBoxSolution, true);
            Solutions.Add(Current);
            listBoxSolutions.Items.Add($"{Algorithm} - {stopwatch.ElapsedMilliseconds} ms - {Current.Last().Level} moves");
            buttonSimulation.Enabled = true;           
            textBoxData.Text = $"{Algorithm} - {stopwatch.ElapsedMilliseconds} ms - {Current.Last().Level} moves";            
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            Array.Clear(Puzzle, 0, Puzzle.Length);
            foreach (Button button in Buttons)
            {
                if (button.Text != "0")
                    Pieces.Add(int.Parse(button.Text));
                button.Text = "0";
            }
            Current = null;
            buttonSimulation.Enabled = false;
        }

        private async void buttonSimulation_Click(object sender, EventArgs e)
        {
            if (Current == null) return;

            EnableTools(false);
            foreach (Node node in Current)
            {
                SetState(node.State);
                await Task.Delay(250);
            }
            await Task.Delay(500);

            Node result = Current.Last();

            SetState();
            await Task.Delay(100);
            SetState(result.State);
            await Task.Delay(100);
            SetState();
            await Task.Delay(100);
            SetState(result.State);
            await Task.Delay(100);
            SetState();
            await Task.Delay(100);

            result = Current.First();

            SetState(result.State);
            EnableTools(true);
        }

        private void comboBoxSetAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            Algorithm = comboBoxMethods.Text;
            buttonSolve.Enabled = true;
        }

        private void listBoxSolutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSolutions.SelectedItem == null)
            {
                buttonDelete.Enabled = false;
                buttonSolve.Enabled = true;
                return;
            }

            Current = Solutions[listBoxSolutions.SelectedIndex];
            PrintSolution(Current, textBoxSolution, true);
            Node start_point = Current.First();
            Array.Copy(start_point.State, Puzzle, Puzzle.Length);
            Pieces.Clear();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Buttons[i, j].Text = Puzzle[i, j].ToString();
                }
            }
            buttonSolve.Enabled = false;
            buttonSimulation.Enabled = true;
            buttonDelete.Enabled = true;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxSolutions.SelectedItem != null)
            {
                Solutions.RemoveAt(listBoxSolutions.SelectedIndex);
                listBoxSolutions.Items.Remove(listBoxSolutions.SelectedItem);
                textBoxSolution.Text = "";
                textBoxData.Text = "";
                buttonSimulation.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetPiece(0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetPiece(0, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetPiece(0, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetPiece(1, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetPiece(1, 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetPiece(1, 2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetPiece(2, 0);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetPiece(2, 1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetPiece(2, 2);
        }

        private bool IsPuzzleSolvable()
        {
            int[] line_of_elements = new int[8];
            int index = 0;
            int inversions = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Puzzle[i, j] == 0)
                        continue;
                    line_of_elements[index++] = Puzzle[i, j];
                }
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    if (line_of_elements[i] > line_of_elements[j])
                        inversions++;
                }
            }

            return inversions % 2 == 0;
        }

        private void SetPiece(int x, int y)
        {
            if (Buttons[x, y].Text == "0")
            {
                Puzzle[x, y] = Pieces.Min;
                Pieces.Remove(Puzzle[x, y]);
                Buttons[x, y].Text = Puzzle[x, y].ToString();
            }
            else
            {
                Pieces.Add(int.Parse(Buttons[x, y].Text));
                Puzzle[x, y] = 0;
                Buttons[x, y].Text = "0";
            }
            buttonSimulation.Enabled = false;
            buttonSolve.Enabled = true;
        }

        private void SetState(int[,] state)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Buttons[i, j].Text = state[i, j].ToString();
                }
            }
        }

        private void SetState()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Buttons[i, j].Text = " ";
                }
            }
        }

        private void EnableTools(bool option)
        {
            foreach (Button button in Buttons)
                button.Enabled = option;
            
            buttonReset.Enabled = option;
            buttonSimulation.Enabled = option;
            listBoxSolutions.Enabled = option;

            if (listBoxSolutions.SelectedItem != null)
                buttonDelete.Enabled = option;
        }

        private void PrintSolution(List<Node> solution, TextBox textBox, bool clearText)
        {
            if (clearText) textBox.Clear();
            foreach (Node node in solution)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                        textBox.AppendText($"{node.State[i, j]} ");
                    textBox.AppendText(Environment.NewLine);
                }
                textBox.AppendText(Environment.NewLine);
            }
        }

        private void buttonToStartPage_Click(object sender, EventArgs e)
        {
            buttonSavePage.Enabled = true;
            panelSecondPage.Visible = false;
            buttonStartPage.Enabled = false;
        }

        private void buttonToSavePage_Click(object sender, EventArgs e)
        {
            buttonStartPage.Enabled = true;
            panelSecondPage.Visible = true;
            buttonSavePage.Enabled = false;

            CopyData(listBoxSolutions, listBoxAddition);
            textBoxContent.Clear();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CopyData(ListBox src, ListBox dst)
        {
            dst.Items.Clear();
            foreach (string item in src.Items)
            {
                dst.Items.Add(item);
            }
        }

        private void listBoxAddition_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxContent.Clear();
            if (listBoxAddition.SelectedIndices.Count == 0)
                buttonSaveAsFile.Enabled = false;
            else
            {
                foreach (int index in listBoxAddition.SelectedIndices)
                {
                    Current = Solutions[index];
                    textBoxContent.AppendText(listBoxAddition.Items[index] + Environment.NewLine);
                    PrintSolution(Current, textBoxContent, false);
                }
                buttonSaveAsFile.Enabled = true;
            }            
        }

        private void checkBoxAllowChanges_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllowChanges.Checked)
                textBoxContent.ReadOnly = false;
            else textBoxContent.ReadOnly = true;
        }

        private void numericUpDownSavePage_ValueChanged(object sender, EventArgs e)
        {
            textBoxContent.Font = new Font("Consolas", (int)numericUpDownSavePage.Value);
        }

        private void numericUpDownStartPage_ValueChanged(object sender, EventArgs e)
        {
            textBoxSolution.Font = new Font("Consolas", (int)numericUpDownStartPage.Value, FontStyle.Bold);            
        }

        private void buttonSaveAsFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxContent.Text))
                buttonSaveAsFile.Enabled = false;
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "8_Puzzle.txt";
                saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
                saveFileDialog.Title = "Зберегти як...";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    File.WriteAllText(filePath, textBoxContent.Text);
                }
            }            
        }

    }
}

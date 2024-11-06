using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Aweal
{
    public partial class MainForm : Form
    {
        int countSchools = 0;
        int countQ = 0;
        int turn = 1;
        private Label lblQuestionCount;
        private SqlConnection connection;
        private ListBox listBoxSchools;
        private Panel panelSeating;
        private ListBox listBoxScores;
        private Label lblQuestion;
        private Label lblSchoolCount;
        private PictureBox pictureBoxQuestion;
        private RadioButton radioButtonA;
        private RadioButton radioButtonB;
        private RadioButton radioButtonC;
        private RadioButton radioButtonD;
        private Timer questionTimer;
        private int timeRemaining;

        private Panel panelStart;
        private Button btnStart;
        private Panel panelMenu;
        private Panel panelSchools;
        private Panel panelQuestions;
        private Panel panelWheel;
        private Panel panelScores;
        private Panel panelQuestionSelection;
        private Panel panelAddQuestion;
      //  private Button btnAddQuestions;
        private Button btnSchools;
        private Button btnQuestions;
        private Button btnWheel;
        private Button btnScores;

        public MainForm()
        {
            InitializeComponent();
            string connectionString = "Data Source=MALAKS-LAPTOP\\SQLEXPRESS;Initial Catalog=AwaeelMusic;Integrated Security=True;Encrypt=False;";
            connection = new SqlConnection(connectionString);

            InitializePanelSchools();
            InitializePanelQuestions();
            InitializePanelWheel();
            InitializePanelScores();
            InitializePanelQuestionSelection();
            LoadSchoolCount(); // Load school count on form load
            LoadQuestionCount(); // Load question count on form load
        }

        private void InitializeComponent()
        {
            this.panelStart = new Panel();
            this.btnStart = new Button();
            this.panelMenu = new Panel();
            this.panelSchools = new Panel();
            this.panelQuestions = new Panel();
            this.panelWheel = new Panel();
            this.panelScores = new Panel();
            this.panelQuestionSelection = new Panel();
            this.panelAddQuestion = new Panel();
            this.btnSchools = new Button();
            this.btnQuestions = new Button();
            this.btnWheel = new Button();
            this.btnScores = new Button();

            // Panel: Start
            this.panelStart.Controls.Add(this.btnStart);
            this.panelStart.Location = new Point(10, 10);
            this.panelStart.Size = new Size(200, 300);

            // Button: Start
            this.btnStart.Text = "Start";
            this.btnStart.Location = new Point(60, 125);
            this.btnStart.Size = new Size(80, 50);
            this.btnStart.Click += new EventHandler(this.btnStart_Click);

            // Panel: Menu
            this.panelMenu.Controls.Add(this.btnSchools);
            this.panelMenu.Controls.Add(this.btnQuestions);
            this.panelMenu.Controls.Add(this.btnWheel);
            this.panelMenu.Controls.Add(this.btnScores);
            this.panelMenu.Location = new Point(10, 10);
            this.panelMenu.Size = new Size(200, 300);
            this.panelMenu.Visible = false;

            // Button: Schools
            this.btnSchools.Text = "Schools";
            this.btnSchools.Location = new Point(10, 10);
            this.btnSchools.Size = new Size(180, 50);
            this.btnSchools.Click += new EventHandler(this.btnSchools_Click);

            // Button: Questions
            this.btnQuestions.Text = "Questions";
            this.btnQuestions.Location = new Point(10, 70);
            this.btnQuestions.Size = new Size(180, 50);
            this.btnQuestions.Click += new EventHandler(this.btnQuestions_Click);

            // Button: Wheel
            this.btnWheel.Text = "Wheel";
            this.btnWheel.Location = new Point(10, 130);
            this.btnWheel.Size = new Size(180, 50);
            this.btnWheel.Click += new EventHandler(this.btnWheel_Click);

            // Button: Scores
            this.btnScores.Text = "Scores";
            this.btnScores.Location = new Point(10, 190);
            this.btnScores.Size = new Size(180, 50);
            this.btnScores.Click += new EventHandler(this.btnScores_Click);

            // Panel: Schools
            this.panelSchools.Location = new Point(220, 10);
            this.panelSchools.Size = new Size(400, 300);
            this.panelSchools.Visible = false;

            // Panel: Questions
            this.panelQuestions.Location = new Point(220, 10);
            this.panelQuestions.Size = new Size(400, 300);
            this.panelQuestions.Visible = false;

            // Panel: Wheel
            this.panelWheel.Location = new Point(220, 10);
            this.panelWheel.Size = new Size(400, 300);
            this.panelWheel.Visible = false;

            // Panel: Scores
            this.panelScores.Location = new Point(220, 10);
            this.panelScores.Size = new Size(400, 300);
            this.panelScores.Visible = false;

            // Panel: Question Selection
            this.panelQuestionSelection.Location = new Point(220, 10);
            this.panelQuestionSelection.Size = new Size(400, 300);
            this.panelQuestionSelection.Visible = false;

            // Dynamically create buttons for each question
            for (int i = 1; i <= countQ; i++) // Assuming there are 10 questions
            {
                Button btnQuestion = new Button
                {
                    Text = $"Question {i}",
                    Name = $"btnQuestion{i}",
                    Location = new Point(10 + ((i - 1) % 2) * 200, 10 + ((i - 1) / 2) * 50),
                    Size = new Size(180, 40)
                };
                btnQuestion.Click += new EventHandler(this.QuestionButton_Click);
                this.panelQuestionSelection.Controls.Add(btnQuestion);
            }

            //add questions
            
            this.panelAddQuestion.Location = new Point(220, 10);
            this.panelAddQuestion.Size = new Size(400, 300);
            this.panelAddQuestion.Visible = false;

            Button btnAddQuestions = new Button { Text = "Add Questions", Location = new Point(10, 250), Size = new Size(180, 50) }; 
            btnAddQuestions.Click += new EventHandler(this.btnAddQuestions_Click);
            this.panelMenu.Controls.Add(btnAddQuestions);

            // Adding Panels to Form
            this.Controls.Add(this.panelStart);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelSchools);
            this.Controls.Add(this.panelQuestions);
            this.Controls.Add(this.panelWheel);
            this.Controls.Add(this.panelScores);
            this.Controls.Add(this.panelQuestionSelection);
            this.Controls.Add(this.panelAddQuestion);



            this.Text = "Quiz Application";
            this.ClientSize = new Size(650, 330);

            // Initialize Timer
            questionTimer = new Timer();
            questionTimer.Interval = 1000; // 1 second intervals
            questionTimer.Tick += new EventHandler(Timer_Tick);
        }

        private void InitializePanelSchools()
        {
            TextBox txtSchoolName = new TextBox { Location = new Point(10, 10), Size = new Size(380, 50) };
            Button btnAddSchool = new Button { Text = "Add School", Location = new Point(10, 40) };
            btnAddSchool.Click += new EventHandler(this.btnAddSchool_Click);
            listBoxSchools = new ListBox { Location = new Point(10, 70), Size = new Size(380, 200) };

            // Add Label for School Count
            lblSchoolCount = new Label { Location = new Point(10, 280), AutoSize = true };
            Button btnBackSchools = new Button { Text = "Back", Location = new Point(100, 270), Size = new Size(80, 30) };
            btnBackSchools.Click += new EventHandler(this.BackButton_Click);

            Button btnResetSchools = new Button { Text = "Reset", Location = new Point(300, 270), Size = new Size(80, 30) }; 
            btnResetSchools.Click += new EventHandler(this.btnResetSchools_Click);

            Button btnDeleteSchool = new Button { Text = "Delete School", Location = new Point(190, 270), Size = new Size(100, 30) }; 
            btnDeleteSchool.Click += new EventHandler(this.btnDeleteSchool_Click);

            this.panelSchools.Controls.Add(txtSchoolName);
            this.panelSchools.Controls.Add(btnAddSchool);
            this.panelSchools.Controls.Add(listBoxSchools);
            this.panelSchools.Controls.Add(btnBackSchools);
            this.panelSchools.Controls.Add(btnResetSchools);
            this.panelSchools.Controls.Add(btnDeleteSchool);
            LoadSchools();
        }
       


        private void InitializePanelQuestions()
        {
            lblQuestion = new Label { Location = new Point(10, 10), AutoSize = true };
            pictureBoxQuestion = new PictureBox { Location = new Point(10, 50), Size = new Size(100, 100) };
            radioButtonA = new RadioButton { Location = new Point(10, 160), AutoSize = true };
            radioButtonB = new RadioButton { Location = new Point(10, 190), AutoSize = true };
            radioButtonC = new RadioButton { Location = new Point(10, 220), AutoSize = true };
            radioButtonD = new RadioButton { Location = new Point(10, 250), AutoSize = true };
            Button btnSubmitAnswer = new Button { Text = "Submit", Location = new Point(10, 280) };
            btnSubmitAnswer.Click += new EventHandler(this.btnSubmitAnswer_Click);

            Button btnBackQuestions = new Button { Text = "Back", Location = new Point(300, 250), Size = new Size(80, 30) };
            btnBackQuestions.Click += new EventHandler(this.BackButton_Click);

            lblQuestionCount = new Label
            {
                Location = new Point(150, 10),
                AutoSize = true
            };

            this.panelQuestions.Controls.Add(lblQuestion);
            this.panelQuestions.Controls.Add(pictureBoxQuestion);
            this.panelQuestions.Controls.Add(radioButtonA);
            this.panelQuestions.Controls.Add(radioButtonB);
            this.panelQuestions.Controls.Add(radioButtonC);
            this.panelQuestions.Controls.Add(radioButtonD);
            this.panelQuestions.Controls.Add(btnSubmitAnswer);
            this.panelQuestions.Controls.Add(lblQuestionCount);
            this.panelQuestions.Controls.Add(btnBackQuestions);
            
        }

        private void InitializePanelWheel()
        {
            Button btnSpinWheel = new Button { Text = "Spin Wheel", Location = new Point(10, 10), Size = new Size(100, 50) };
            btnSpinWheel.Click += new EventHandler(this.btnSpinWheel_Click);
            panelSeating = new Panel { Location = new Point(10, 70), Size = new Size(380, 240) };
            Button btnBackWheel = new Button { Text = "Back", Location = new Point(300, 250), Size = new Size(80, 30) };
            btnBackWheel.Click += new EventHandler(this.BackButton_Click);

            this.panelWheel.Controls.Add(btnSpinWheel);
            this.panelWheel.Controls.Add(panelSeating);
            this.panelWheel.Controls.Add(btnBackWheel);

        }

        private void InitializePanelScores()
        {
            Label lblScores = new Label { Text = "Scores", Location = new Point(10, 10), AutoSize = true };
            listBoxScores = new ListBox { Location = new Point(10, 40), Size = new Size(380, 240) };

            Button btnBackScores = new Button { Text = "Back", Location = new Point(300, 250), Size = new Size(80, 30) }; 
            btnBackScores.Click += new EventHandler(this.BackButton_Click);

            this.panelScores.Controls.Add(lblScores);
            this.panelScores.Controls.Add(listBoxScores);
            this.panelScores.Controls.Add(btnBackScores);
        }

        private void InitializePanelQuestionSelection()
        {
            // Panel initialization and question buttons are already handled in InitializeComponent
            Button btnBackQuestionSelection = new Button { Text = "Back", Location = new Point(300, 250), Size = new Size(80, 30) }; 
            btnBackQuestionSelection.Click += new EventHandler(this.BackButton_Click); 
            this.panelQuestionSelection.Controls.Add(btnBackQuestionSelection); 
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.panelStart.Visible = false;
            this.panelMenu.Visible = true;
        }

        private void btnSchools_Click(object sender, EventArgs e)
        {
            this.panelMenu.Visible = false;
            this.panelSchools.Visible = true;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = false;
            this.panelScores.Visible = false;
            this.panelQuestionSelection.Visible = false;
        }
        private void btnResetSchools_Click(object sender, EventArgs e)
        {
            string deleteQuery = "DELETE FROM Schools";
            string resetIdQuery = "DBCC CHECKIDENT ('Schools', RESEED, 0)";

            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
            }

            using (SqlCommand command = new SqlCommand(resetIdQuery, connection))
            {
                command.ExecuteNonQuery();
                CloseConnection();
            }

            LoadSchools();
            LoadSchoolCount();
        }
        private void LoadQuestionCount()
        {
            string query = "SELECT COUNT(*) FROM Questions";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int questionCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblQuestionCount.Text = $"Number of Questions: {questionCount}";
                countQ = questionCount;
            }
        }
        private void btnAddQuestions_Click(object sender, EventArgs e)
        {
            this.panelMenu.Visible = false;
            this.panelAddQuestion.Visible = true;
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ((TextBox)this.panelAddQuestion.Controls["txtQuestionImage"]).Text = openFileDialog.FileName;
            }
        }

        private void btnBrowseAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ((TextBox)this.panelAddQuestion.Controls["txtQuestionAudio"]).Text = openFileDialog.FileName;
            }
        }

        private void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            string questionText = ((TextBox)this.panelAddQuestion.Controls["txtQuestionText"]).Text;
            byte[] questionImage = File.ReadAllBytes(((TextBox)this.panelAddQuestion.Controls["txtQuestionImage"]).Text);
            byte[] questionAudio = File.ReadAllBytes(((TextBox)this.panelAddQuestion.Controls["txtQuestionAudio"]).Text);

            // Get the options similarly...

            string query = "INSERT INTO Questions (QuestionText, QuestionImage, QuestionAudio, ... other fields ...) VALUES (@QuestionText, @QuestionImage, @QuestionAudio, ... other parameters ...)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionText", (object)questionText ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionImage", (object)questionImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionAudio", (object)questionAudio ?? DBNull.Value);

                // Add parameters for options...

                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            MessageBox.Show("Question saved successfully.");
        }

        private void btnDeleteSchool_Click(object sender, EventArgs e)
        {
            if (listBoxSchools.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)listBoxSchools.SelectedItem;
                string selectedSchool = selectedRow["Name"].ToString();

                string query = "DELETE FROM Schools WHERE Name = @Name";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", selectedSchool);
                    OpenConnection();
                    int result = command.ExecuteNonQuery();
                    CloseConnection();

                    if (result > 0)
                    {
                        MessageBox.Show("School deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the school. Please try again.");
                    }
                }

                LoadSchools();
                LoadSchoolCount();
            }
            else
            {
                MessageBox.Show("Please select a school to delete.");
            }
        }




        private void btnQuestions_Click(object sender, EventArgs e)
        {
            this.panelMenu.Visible = false;
            this.panelSchools.Visible = false;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = false;
            this.panelScores.Visible = false;
            this.panelQuestionSelection.Visible = true;
        }

        private void btnWheel_Click(object sender, EventArgs e)
        {
            this.panelMenu.Visible = false;
            this.panelSchools.Visible = false;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = true;
            this.panelScores.Visible = false;
            this.panelQuestionSelection.Visible = false;
        }

        private void btnScores_Click(object sender, EventArgs e)
        {
            this.panelMenu.Visible = false;
            this.panelSchools.Visible = false;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = false;
            this.panelScores.Visible = true;
            this.panelQuestionSelection.Visible = false;
            LoadScores();
        }

        private void QuestionButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int questionNumber = int.Parse(clickedButton.Text.Replace("Question ", ""));
            DisplayQuestion(questionNumber);
            StartTimer();
            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }

        private void StartTimer()
        {
            timeRemaining = 30; // 30 seconds to answer
            questionTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeRemaining--;

            if (timeRemaining <= 0)
            {
                questionTimer.Stop();
                MessageBox.Show("Time's up!");
                // Handle what happens when time runs out, e.g., show the next question or switch to the next school
            }
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.panelSchools.Visible = false;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = false;
            this.panelScores.Visible = false;
            this.panelQuestionSelection.Visible = false;
            this.panelMenu.Visible = true;
        }

        private void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            string selectedAnswer = string.Empty;
            if (radioButtonA.Checked) selectedAnswer = "A";
            if (radioButtonB.Checked) selectedAnswer = "B";
            if (radioButtonC.Checked) selectedAnswer = "C";
            if (radioButtonD.Checked) selectedAnswer = "D";

            int questionNumber = int.Parse(lblQuestion.Text.Replace("Question ", "").Split(':')[0]); // Assuming question label format is "Question {number}: ..."
            int schoolID = turn; // Implement method to get current school's ID
            turn++;
            if (turn > countSchools) turn = 1;
            bool isCorrect = CheckAnswer(selectedAnswer, questionNumber);

            if (isCorrect)
            {
                UpdateScores(schoolID, 1); // Increment score by 1
            }

            MarkQuestionAsAnswered(questionNumber);
            DisplayNextQuestion(); // Implement this method to display the next question
        }
        private void LoadSchoolCount()
        {
            string query = "SELECT COUNT(*) FROM Schools";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int schoolCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblSchoolCount.Text = $"Number of Schools: {schoolCount}";
                countSchools = schoolCount;
            }
        }

        private void MarkQuestionAsAnswered(int questionNumber)
        {
            string query = "UPDATE Questions SET IsAnswered = 1 WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void DisplayNextQuestion()
        {
            this.panelQuestions.Visible = false;
            this.panelQuestionSelection.Visible = true;
        }

        private void btnAddSchool_Click(object sender, EventArgs e)
        {
            string schoolName = ((TextBox)this.panelSchools.Controls[0]).Text;
            string query = "INSERT INTO Schools (Name) VALUES (@Name)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", schoolName);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            LoadSchools();
            LoadSchoolCount(); // Update school count
        }

        private void LoadSchools()
        {
            string query = "SELECT Name FROM Schools";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            listBoxSchools.DataSource = table;
            listBoxSchools.DisplayMember = "Name";
        }

        private void btnSpinWheel_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int index = random.Next(listBoxSchools.Items.Count);
            string selectedSchool = listBoxSchools.Items[index].ToString();

            MessageBox.Show("Selected School: " + selectedSchool);
            ArrangeSeating(selectedSchool);
        }

        private void ArrangeSeating(string selectedSchool)
        {
            panelSeating.Controls.Clear();
            int numberOfSeats = 10;

            for (int i = 1; i <= numberOfSeats; i++)
            {
                Label seatLabel = new Label
                {
                    Text = $"Seat {i}",
                    AutoSize = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    Padding = new Padding(5),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                panelSeating.Controls.Add(seatLabel);
            }

            int schoolID = GetSchoolID(selectedSchool); // Implement this method to get the school ID based on the name
            SaveSeatingArrangement(schoolID);

            MessageBox.Show($"Seating arranged for {selectedSchool}");
        }

        private int GetSchoolID(string schoolName)
        {
            string query = "SELECT ID FROM Schools WHERE Name = @Name";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", schoolName);
                OpenConnection();
                int schoolID = (int)command.ExecuteScalar();
                CloseConnection();
                return schoolID;
            }
        }


        private void SaveSeatingArrangement(int schoolID)
        {
            string query = "INSERT INTO SeatingArrangements (SchoolID, SeatNumber) VALUES (@SchoolID, @SeatNumber)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                for (int seatNumber = 1; seatNumber <= 10; seatNumber++)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SchoolID", schoolID);
                    command.Parameters.AddWithValue("@SeatNumber", seatNumber);
                    OpenConnection();
                    command.ExecuteNonQuery();
                    CloseConnection();
                }
            }
        }


        private void DisplayQuestion(int questionNumber)
        {
            string query = "SELECT * FROM Questions WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Display text
                    if (reader["QuestionText"] != DBNull.Value)
                    {
                        lblQuestion.Text = reader["QuestionText"].ToString();
                    }

                    // Display image
                    if (reader["QuestionImage"] != DBNull.Value)
                    {
                        byte[] imageData = (byte[])reader["QuestionImage"];
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            pictureBoxQuestion.Image = Image.FromStream(ms);
                        }
                    }

                    // Play audio
                    if (reader["QuestionAudio"] != DBNull.Value)
                    {
                        byte[] audioData = (byte[])reader["QuestionAudio"];
                        using (MemoryStream ms = new MemoryStream(audioData))
                        {
                            SoundPlayer player = new SoundPlayer(ms);
                            player.Play();
                        }
                    }

                    // Display options
                    if (reader["OptionA"] != DBNull.Value) radioButtonA.Text = reader["OptionA"].ToString();
                    if (reader["OptionB"] != DBNull.Value) radioButtonB.Text = reader["OptionB"].ToString();
                    if (reader["OptionC"] != DBNull.Value) radioButtonC.Text = reader["OptionC"].ToString();
                    if (reader["OptionD"] != DBNull.Value) radioButtonD.Text = reader["OptionD"].ToString();
                }

                reader.Close();
                CloseConnection();
            }
        }

        private bool CheckAnswer(string selectedAnswer, int questionNumber)
        {
            string query = "SELECT CorrectAnswer FROM Questions WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                string correctAnswer = command.ExecuteScalar().ToString();
                CloseConnection();

                return selectedAnswer == correctAnswer;
            }
        }



        private void UpdateScores(int schoolID, int scoreToAdd)
        {
            string query = "UPDATE Scores SET Score = Score + @ScoreToAdd WHERE SchoolID = @SchoolID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ScoreToAdd", scoreToAdd);
                command.Parameters.AddWithValue("@SchoolID", schoolID);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void LoadScores()
        {
            string query = "SELECT s.Name, sc.Score FROM Schools s INNER JOIN Scores sc ON s.ID = sc.SchoolID ORDER BY sc.Score DESC";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            listBoxScores.DataSource = table;
            listBoxScores.DisplayMember = "Name";
            listBoxScores.ValueMember = "Score";
        }

        private void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

    }
}

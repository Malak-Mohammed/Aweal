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
        private Button btnAddQuestions;
        private Button btnSchools;
        private Button btnQuestions;
        private Button btnWheel;
        private Button btnScores;
        //private TextBox txtQuestionImage;
        //private TextBox txtQuestionAudio;
        //private TextBox txtOptionAImage;
        //private TextBox txtOptionBImage;
        //private TextBox txtOptionCImage;
        //private TextBox txtOptionDImage;
        //private TextBox txtOptionAAudio;
        //private TextBox txtOptionBAudio;
        //private TextBox txtOptionCAudio;
        //private TextBox txtOptionDAudio;

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
            InitializePanelAddQuestions();
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
            //LoadQuestionCount();
          

            //add questions
            
            this.panelAddQuestion.Location = new Point(220, 10);
            this.panelAddQuestion.Size = new Size(900,900);
            this.panelAddQuestion.Visible = false;

            Button btnAddQuestions = new Button { Text = "Add Questions", Location = new Point(10, 250), Size = new Size(180, 50) }; 
            btnAddQuestions.Click += new EventHandler(this.btnAddQuestions_Click);
            this.panelMenu.Controls.Add(btnAddQuestions);

            //InitializePanelAddQuestions();

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
            LoadQuestionCount();

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
            LoadQuestionCount();
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
            Button btnBackQuestionSelection = new Button { Text = "Back", Location = new Point(300, 250), Size = new Size(80, 30) }; 
            btnBackQuestionSelection.Click += new EventHandler(this.BackButton_Click); 
            this.panelQuestionSelection.Controls.Add(btnBackQuestionSelection); 
        }
        private void InitializePanelAddQuestions()
        {
            

            // Question Text
            Label lblQuestionText = new Label { Text = "Question Text", Location = new Point(10, 10) };
            TextBox txtQuestionText = new TextBox { Location = new Point(100, 10), Width = 200 };

            // Question Image
            Label lblQuestionImage = new Label { Text = "Question Image", Location = new Point(10, 40) };
            TextBox txtQuestionImage = new TextBox { Location = new Point(100, 40), Width = 200 };
            Button btnBrowseImage = new Button { Text = "Browse", Location = new Point(310, 40) };
            btnBrowseImage.Click += new EventHandler(this.btnBrowseImage_Click);

            // Question Audio
            Label lblQuestionAudio = new Label { Text = "Question Audio", Location = new Point(10, 70) };
            TextBox txtQuestionAudio = new TextBox { Location = new Point(100, 70), Width = 200 };
            Button btnBrowseAudio = new Button { Text = "Browse", Location = new Point(310, 70) };
            btnBrowseAudio.Click += new EventHandler(this.btnBrowseAudio_Click);

            // Option A
            Label lblOptionA = new Label { Text = "Option A", Location = new Point(10, 100) };
            TextBox txtOptionAText = new TextBox { Location = new Point(100, 100), Width = 200 };
            TextBox txtOptionAImage = new TextBox { Location = new Point(100, 130), Width = 200 };
            Button btnBrowseOptionAImage = new Button { Text = "Browse", Location = new Point(310, 130) };
            btnBrowseOptionAImage.Click += new EventHandler(this.btnBrowseOptionAImage_Click);
            TextBox txtOptionAAudio = new TextBox { Location = new Point(100, 160), Width = 200 };
            Button btnBrowseOptionAAudio = new Button { Text = "Browse", Location = new Point(310, 160) };
            btnBrowseOptionAAudio.Click += new EventHandler(this.btnBrowseOptionAAudio_Click);
            RadioButton rbOptionA = new RadioButton
            {
                Text = "Correct",
                Location = new Point(350, 100),
                Name = "rbOptionA"
            };
            rbOptionA.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddQuestion.Controls.Add(rbOptionA);


            // Option B
            Label lblOptionB = new Label { Text = "Option B", Location = new Point(10, 190) };
            TextBox txtOptionBText = new TextBox { Location = new Point(100, 190), Width = 200 };
            TextBox txtOptionBImage = new TextBox { Location = new Point(100, 220), Width = 200 };
            Button btnBrowseOptionBImage = new Button { Text = "Browse", Location = new Point(310, 220) };
            btnBrowseOptionBImage.Click += new EventHandler(this.btnBrowseOptionBImage_Click);
            TextBox txtOptionBAudio = new TextBox { Location = new Point(100, 250), Width = 200 };
            Button btnBrowseOptionBAudio = new Button { Text = "Browse", Location = new Point(310, 250) };
            btnBrowseOptionBAudio.Click += new EventHandler(this.btnBrowseOptionBAudio_Click);
            RadioButton rbOptionB = new RadioButton
            {
                Text = "Correct",
                Location = new Point(350, 190),
                Name = "rbOptionB"
            };
            rbOptionB.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddQuestion.Controls.Add(rbOptionB);


            // Option C
            Label lblOptionC = new Label { Text = "Option C", Location = new Point(10, 280) };
            TextBox txtOptionCText = new TextBox { Location = new Point(100, 280), Width = 200 };
            TextBox txtOptionCImage = new TextBox { Location = new Point(100, 310), Width = 200 };
            Button btnBrowseOptionCImage = new Button { Text = "Browse", Location = new Point(310, 310) };
            btnBrowseOptionCImage.Click += new EventHandler(this.btnBrowseOptionCImage_Click);
            TextBox txtOptionCAudio = new TextBox { Location = new Point(100, 340), Width = 200 };
            Button btnBrowseOptionCAudio = new Button { Text = "Browse", Location = new Point(310, 340) };
            btnBrowseOptionCAudio.Click += new EventHandler(this.btnBrowseOptionCAudio_Click);
            RadioButton rbOptionC = new RadioButton
            {
                Text = "Correct",
                Location = new Point(350, 280),
                Name = "rbOptionC"
            };
            rbOptionC.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddQuestion.Controls.Add(rbOptionC);


            // Option D
            Label lblOptionD = new Label { Text = "Option D", Location = new Point(10, 370) };
            TextBox txtOptionDText = new TextBox { Location = new Point(100, 370), Width = 200 };
            TextBox txtOptionDImage = new TextBox { Location = new Point(100, 400), Width = 200 };
            Button btnBrowseOptionDImage = new Button { Text = "Browse", Location = new Point(310, 400) };
            btnBrowseOptionDImage.Click += new EventHandler(this.btnBrowseOptionDImage_Click);
            TextBox txtOptionDAudio = new TextBox { Location = new Point(100, 430), Width = 200 };
            Button btnBrowseOptionDAudio = new Button { Text = "Browse", Location = new Point(310, 430) };
            btnBrowseOptionDAudio.Click += new EventHandler(this.btnBrowseOptionDAudio_Click);
            RadioButton rbOptionD = new RadioButton
            {
                Text = "Correct",
                Location = new Point(350, 370),
                Name = "rbOptionD"
            };
            rbOptionD.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddQuestion.Controls.Add(rbOptionD);


            // Save Button
            Button btnSaveQuestion = new Button { Text = "Save Question", Location = new Point(150, 490) };
            btnSaveQuestion.Click += new EventHandler(this.btnSaveQuestion_Click);

            // Back Button
            Button btnBackAddQuestions = new Button { Text = "Back", Location = new Point(300, 470), Size = new Size(80, 30) };
            btnBackAddQuestions.Click += new EventHandler(this.BackButton_Click);

            // Add Controls to Panel
            this.panelAddQuestion.Controls.Add(lblQuestionText);
            this.panelAddQuestion.Controls.Add(txtQuestionText);
            this.panelAddQuestion.Controls.Add(lblQuestionImage);
            this.panelAddQuestion.Controls.Add(txtQuestionImage);//7
            this.panelAddQuestion.Controls.Add(btnBrowseImage);//8
            this.panelAddQuestion.Controls.Add(lblQuestionAudio);//9
            this.panelAddQuestion.Controls.Add(txtQuestionAudio);//10
            this.panelAddQuestion.Controls.Add(btnBrowseAudio);//11
            this.panelAddQuestion.Controls.Add(lblOptionA);//12
            this.panelAddQuestion.Controls.Add(txtOptionAText);//13
            this.panelAddQuestion.Controls.Add(txtOptionAImage);//14
            this.panelAddQuestion.Controls.Add(btnBrowseOptionAImage);//15
            this.panelAddQuestion.Controls.Add(txtOptionAAudio);//16
            this.panelAddQuestion.Controls.Add(btnBrowseOptionAAudio);
            this.panelAddQuestion.Controls.Add(lblOptionB);
            this.panelAddQuestion.Controls.Add(txtOptionBText);//18
            this.panelAddQuestion.Controls.Add(txtOptionBImage);//19
            this.panelAddQuestion.Controls.Add(btnBrowseOptionBImage);//20
            this.panelAddQuestion.Controls.Add(txtOptionBAudio);//21
            this.panelAddQuestion.Controls.Add(btnBrowseOptionBAudio);//22
            this.panelAddQuestion.Controls.Add(lblOptionC);//23
            this.panelAddQuestion.Controls.Add(txtOptionCText);//24
            this.panelAddQuestion.Controls.Add(txtOptionCImage);//25
            this.panelAddQuestion.Controls.Add(btnBrowseOptionCImage);//26
            this.panelAddQuestion.Controls.Add(txtOptionCAudio);//27
            this.panelAddQuestion.Controls.Add(btnBrowseOptionCAudio);//28
            this.panelAddQuestion.Controls.Add(lblOptionD);//29
            this.panelAddQuestion.Controls.Add(txtOptionDText);//30
            this.panelAddQuestion.Controls.Add(txtOptionDImage);//31
            this.panelAddQuestion.Controls.Add(btnBrowseOptionDImage);//32
            this.panelAddQuestion.Controls.Add(txtOptionDAudio);//33
            this.panelAddQuestion.Controls.Add(btnBrowseOptionDAudio);
            this.panelAddQuestion.Controls.Add(btnSaveQuestion);
            this.panelAddQuestion.Controls.Add(btnBackAddQuestions);
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton.Checked)
            {
                foreach (Control control in this.panelAddQuestion.Controls)
                {
                    if (control is RadioButton radioButton && radioButton != selectedRadioButton)
                    {
                        radioButton.Checked = false;
                    }
                }
            }
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
                // Set the text of the textbox to the selected file path
                TextBox txtQuestionImage = (TextBox)this.panelAddQuestion.Controls[7];
                if (txtQuestionImage != null)
                {
                    txtQuestionImage.Text = openFileDialog.FileName;

                }
                else
                {
                    MessageBox.Show("Textbox for Question Image not found.");
                }
            }
        }
        private void btnBrowseOptionAImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAImage = (TextBox)this.panelAddQuestion.Controls[14];
                if (txtOptionAImage != null)
                {
                    txtOptionAImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option A Image not found.");
                }
            }
        }

        private void btnBrowseOptionBImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBImage = (TextBox)this.panelAddQuestion.Controls[20];
                if (txtOptionBImage != null)
                {
                    txtOptionBImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option B Image not found.");
                }
            }
        }

        private void btnBrowseOptionCImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCImage = (TextBox)this.panelAddQuestion.Controls[26];
                if (txtOptionCImage != null)
                {
                   txtOptionCImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option C Image not found.");
                }
            }
        }

        private void btnBrowseOptionDImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDImage = (TextBox)this.panelAddQuestion.Controls[31];
                if (txtOptionDImage != null)
                {
                    txtOptionDImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option D Image not found.");
                }
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
                TextBox txtQuestionAudio = (TextBox)this.panelAddQuestion.Controls[10];
                if (txtQuestionAudio != null)
                {
                    txtQuestionAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Question Audio not found.");
                }
            }
        }


        private void btnBrowseOptionAAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAAudio = (TextBox)this.panelAddQuestion.Controls[16];
                if (txtOptionAAudio != null)
                {
                    txtOptionAAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option A Audio not found.");
                }
            }
        }

        private void btnBrowseOptionBAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBAudio = (TextBox)this.panelAddQuestion.Controls[22];
                if (txtOptionBAudio != null)
                {
                    txtOptionBAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option B Audio not found.");
                }
            }
        }

        private void btnBrowseOptionCAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCAudio = (TextBox)this.panelAddQuestion.Controls[28];
                if (txtOptionCAudio != null)
                {
                    txtOptionCAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option C Audio not found.");
                }
            }
        }

        private void btnBrowseOptionDAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDAudio = (TextBox)this.panelAddQuestion.Controls[34];
                if (txtOptionDAudio != null)
                {
                   txtOptionDAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Option D Audio not found.");
                }
            }
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


        private void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            string questionText = ((TextBox)this.panelAddQuestion.Controls[5]).Text;

            byte[] questionImage = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[7])?.Text);
            byte[] questionAudio = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[10])?.Text);

            // Option A
            string optionAText = ((TextBox)this.panelAddQuestion.Controls[13]).Text;
            byte[] optionAImage = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[14])?.Text);
            byte[] optionAAudio = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[16])?.Text);

            // Option B
            string optionBText = ((TextBox)this.panelAddQuestion.Controls[19]).Text;
            byte[] optionBImage = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[20])?.Text);
            byte[] optionBAudio = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[22])?.Text);

            // Option C
            string optionCText = ((TextBox)this.panelAddQuestion.Controls[25]).Text;
            byte[] optionCImage = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[26])?.Text);
            byte[] optionCAudio = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[28])?.Text);

            // Option D
            string optionDText = ((TextBox)this.panelAddQuestion.Controls[31]).Text;
            byte[] optionDImage = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[32])?.Text);
            byte[] optionDAudio = GetFileDataSafe(((TextBox)this.panelAddQuestion.Controls[34])?.Text);

            // Determine Correct Answer
            string correctAnswer = string.Empty;
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionA"]).Checked) correctAnswer = "A";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionB"]).Checked) correctAnswer = "B";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionC"]).Checked) correctAnswer = "C";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionD"]).Checked) correctAnswer = "D";

            string query = "INSERT INTO Questions (QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer) " +
                           "VALUES (@QuestionText, CONVERT(VARBINARY(MAX), @QuestionImage), CONVERT(VARBINARY(MAX), @QuestionAudio), @OptionAText, CONVERT(VARBINARY(MAX), @OptionAImage), CONVERT(VARBINARY(MAX), @OptionAAudio), @OptionBText, CONVERT(VARBINARY(MAX), @OptionBImage), CONVERT(VARBINARY(MAX), @OptionBAudio), @OptionCText, CONVERT(VARBINARY(MAX), @OptionCImage), CONVERT(VARBINARY(MAX), @OptionCAudio), @OptionDText, CONVERT(VARBINARY(MAX), @OptionDImage), CONVERT(VARBINARY(MAX), @OptionDAudio), @CorrectAnswer)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionText", (object)questionText ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionImage", (object)questionImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionAudio", (object)questionAudio ?? DBNull.Value);

                command.Parameters.AddWithValue("@OptionAText", (object)optionAText ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionAImage", (object)optionAImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionAAudio", (object)optionAAudio ?? DBNull.Value);

                command.Parameters.AddWithValue("@OptionBText", (object)optionBText ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionBImage", (object)optionBImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionBAudio", (object)optionBAudio ?? DBNull.Value);

                command.Parameters.AddWithValue("@OptionCText", (object)optionCText ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionCImage", (object)optionCImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionCAudio", (object)optionCAudio ?? DBNull.Value);

                command.Parameters.AddWithValue("@OptionDText", (object)optionDText ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionDImage", (object)optionDImage ?? DBNull.Value);
                command.Parameters.AddWithValue("@OptionDAudio", (object)optionDAudio ?? DBNull.Value);

                command.Parameters.AddWithValue("@CorrectAnswer", (object)correctAnswer ?? DBNull.Value);

                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            MessageBox.Show("Question saved successfully.");
            LoadQuestionCount();    
        }

        private byte[] GetFileDataSafe(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    return File.ReadAllBytes(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading file data: {ex.Message}\n{ex.StackTrace}");
            }
            return null;
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
            this.panelAddQuestion.Visible = false;
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;



namespace Aweal
{
    public partial class MainForm : Form
    {
        int countSchools = 0;
        int countQ = 0;
        int countHQ = 0;
        int countRQ = 0;
        int turn = 1;
        int chosenQ = 0;
        private int seatNumber = 1;
        private SoundPlayer warningSoundPlayer;

        private List<int> seatingOrder;
        private int currentSeat = 0;

        private SoundPlayer soundPlayer;
        private Timer musicLoopTimer;
        private QuestionType currentQuestionType;


        private Panel panelDaySelection;
        private Button[] dayButtons;
        private int selectedDay = 1; // Default to Day 1

        private Label lblQuestionCount;
        private SqlConnection connection;
        private ListBox listBoxSchools;
        private Panel panelSeating;
        private ListBox listBoxScores;
        private Label lblQuestion;
        private Label lblSchoolCount;
        private Label lblTime;
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
        private Button btnViewQuestions;
        private Button btnWheel;
        private Button btnScores;
        private Button btnAddHearingQuestions;
        private Button btnAddReadingQuestions;
        private Button btnStartQuiz;
        private WheelControl wheelControl;
        private Panel panelAddHearingQuestion;
        private Panel panelAddReadingQuestion;
        private Timer hearingQuestionTimer;
        private int hearingTimeRemaining;

        private Timer readingQuestionTimer;
        private int readingTimeRemaining;
        private bool isQuizMode = false;
        private Dictionary<string, int> scores = new Dictionary<string, int>();

       

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

            this.Text = "Quiz Application";
            this.Size = new Size(1200, 800);
            this.Paint += new PaintEventHandler(Form_Paint);

            InitializePanelDaySelection();
            InitializePanelSchools();
            InitializePanelQuestions();
            InitializePanelWheel();
            InitializePanelScores();
            InitializePanelQuestionSelection();
            InitializePanelAddQuestions();
            InitializeTimer();
            InitializeCountdownLabel();
            InitializePanelAddReadingQuestions();
            InitializePanelAddHearingQuestions();
            LoadSchoolCount();
            LoadQuestionCount();
            LoadHearingQuestionCount();
            LoadHearingQuestionCount();
            LoadSchoolsIntoWheel();
            InitializeTimers();
           // InitializeFrontPanel();
         

            // Initially hide all panels except the start panel
            panelDaySelection.Visible = false;
            panelWheel.Visible = false;
            panelMenu.Visible = false; 
            panelSchools.Visible = false;
            panelQuestions.Visible = false;
            panelScores.Visible = false;
            panelQuestionSelection.Visible = false;
            panelAddQuestion.Visible = false;
            panelAddHearingQuestion.Visible = false;
            panelAddReadingQuestion.Visible = false;
          
            // Set form to full screen
            this.WindowState = FormWindowState.Maximized;
            
        }

        private void InitializeComponent()
        {
            this.panelStart = new Panel();
            this.panelDaySelection = new Panel();
            this.btnStart = new Button();
            this.panelMenu = new Panel();
            this.panelSchools = new Panel();
            this.panelQuestions = new Panel();
            this.panelWheel = new Panel();
            this.panelScores = new Panel();
            this.panelQuestionSelection = new Panel();
            this.panelAddQuestion = new Panel();
            this.btnSchools = new Button();
            this.btnViewQuestions = new Button();
            this.btnAddHearingQuestions = new Button();
            this.btnAddReadingQuestions = new Button();
            this.btnStartQuiz = new Button();
            this.btnWheel = new Button();
            this.btnScores = new Button();
            this.panelAddHearingQuestion = new Panel();
            this.panelAddReadingQuestion = new Panel(); 

            

            // Panel: Start
            this.panelStart.Controls.Add(this.btnStart);
            this.panelStart.Location = new Point(10, 10);
            this.panelStart.Size = new Size(200, 300);

            // Button: Start
            this.btnStart.Text = "Start";
            this.btnStart.Location = new Point(60, 125);
            this.btnStart.Size = new Size(80, 50);
            this.btnStart.Click += new EventHandler(this.btnStart_Click);

            // Panel: Day Selection
            this.panelDaySelection.Location = new Point(10, 10);
            this.panelDaySelection.Size = new Size(400, 400); 
            this.panelDaySelection.Visible = false;


            // Panel: Menu
            this.panelMenu.Controls.Add(this.btnSchools);
            this.panelMenu.Controls.Add(this.btnViewQuestions);
            this.panelMenu.Controls.Add(this.btnWheel);
            this.panelMenu.Controls.Add(this.btnScores);
            this.panelMenu.Controls.Add(this.btnAddHearingQuestions);
            this.panelMenu.Controls.Add(this.btnAddReadingQuestions);
            this.panelMenu.Controls.Add(btnStartQuiz);
            this.panelMenu.Dock = DockStyle.Fill;
            this.panelMenu.Location = new Point(10, 10);
            this.panelMenu.BackColor= Color.Transparent;
            this.panelMenu.Visible = false;

           
            // Button: Schools
            this.btnSchools.Text = "Schools";
            this.btnSchools.Location = new Point(10, 10);
            this.btnSchools.Size = new Size(180, 50);
            this.btnSchools.Click += new EventHandler(this.btnSchools_Click);

            // Button: View Questions
            this.btnViewQuestions.Text = "View Questions";
            this.btnViewQuestions.Location = new Point(10, 70);
            this.btnViewQuestions.Size = new Size(180, 50);
            this.btnViewQuestions.Click += new EventHandler(this.btnViewQuestions_Click);

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
            

            this.btnAddHearingQuestions.Text = "Add Hearing Questions";
            this.btnAddHearingQuestions.Location = new Point(10, 250);
            this.btnAddHearingQuestions.Size = new Size(180, 50);
          
            this.btnAddHearingQuestions.Click += new EventHandler(this.btnAddHearingQuestions_Click);



            this.btnAddReadingQuestions.Text = "Add Reading Questions";
            this.btnAddReadingQuestions.Location = new Point(10, 310);
            this.btnAddReadingQuestions.Size = new Size(180, 50);
           
           this.btnAddReadingQuestions.Click += new EventHandler(this.btnAddReadingQuestions_Click);

            //Button start quiz 
            this.btnStartQuiz.Text = "Start Quiz";
            this.btnStartQuiz.Location = new Point(10, 370);
            this.btnStartQuiz.Size = new Size(180, 50);
            this.btnStartQuiz.Click += new EventHandler(this.btnStartQuiz_Click);


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
            this.panelWheel.Size = new Size(900,900);
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
            this.Controls.Add(this.panelDaySelection);




            


            // Initialize Timer
            questionTimer = new Timer();
            questionTimer.Interval = 1000; // 1 second intervals
            questionTimer.Tick += new EventHandler(Timer_Tick);
        }
  
    private void InitializeFrontPanel()
    {
        // Create the front panel
        Panel frontPanel = new Panel
        {
            Name = "frontPanel",
            Dock = DockStyle.Fill,
           BackColor=Color.Transparent
        };
            this.Controls.Add(frontPanel);

        // Create the logo (top left)
        PictureBox logoPictureBox = new PictureBox
        {
            Image = Properties.Resources.logo, // Use the logo image from resources
            Size = new Size((int)(2.84 * 96), (int)(2.47 * 96)),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Location = new Point(10, 10) // Top left corner
        };
        frontPanel.Controls.Add(logoPictureBox);

        // Create the additional label (top right)
        Label topRightLabel = new Label
        {
            Text = "سلطنة عُمان\r\nالمديرية العامة التربية والتعليم\r\nبمحافظة شمال الباطنة\r\nدائرة الإشراف التربوي\r\nقسم الإشراف الفني\r\nوحدة المهارات الموسيقية\r\n", // Set your desired text
            Font = new Font("Arial", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(frontPanel.Width - 300, 10), // Adjust location as needed
            ForeColor = Color.Black,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter
        };
        frontPanel.Controls.Add(topRightLabel);

             int pictureWidth = (int)(5.21 * 96);
            int pictureHeight = (int)(5.21 * 96);
            PictureBox pictureBox = new PictureBox
        {
            Image = Properties.Resources.front, // Use the image from resources
                Size = new Size(pictureWidth, pictureHeight), 
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location = new Point((frontPanel.Width - pictureWidth) / 2, 60)
            };
        frontPanel.Controls.Add(pictureBox);
            pictureBox.SendToBack();

            //// Load the custom font from resources
            //PrivateFontCollection privateFonts = new PrivateFontCollection();
            //byte[] fontData = Properties.Resources.KFGQPC_Uthmanic_Script; // Use the font from resources
            //IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            //Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            //privateFonts.AddMemoryFont(fontPtr, fontData.Length);
            //Marshal.FreeCoTaskMem(fontPtr);
            //Font customFont = new Font(privateFonts.Families[0], 24, FontStyle.Bold); // Adjust size and style as needed

            // Create the title label (with white background)
            Label lblTitle = new Label
            {
                Text = "مشروع أوائل الطلبة المجيدين في المهارات الموسيقية الفصل الدراسي الثاني 2024-2025\r\nتحت رعاية\r\nالفاضل/سيف بن حارب بن مسعود الغافري\r\nالمدير العام المساعد للشؤن الإدارية والمالية \r\n", // Your Arabic title
                Font = new Font("Aldhabi", 44, FontStyle.Bold),
                Size = new Size(1143, 270),
                BackColor = Color.White, 
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                

            };
        
        frontPanel.Controls.Add(lblTitle);
            lblTitle.BringToFront();
            lblTitle.Location = new Point((frontPanel.Width - lblTitle.Width) / 2, 350);
            
            // Create the Start button
            Button btnStartFront = new Button
        {
            Text = "بدء", 
            Size = new Size(150, 50),
            Location = new Point((frontPanel.Width - 150) / 2, lblTitle.Bottom + 20),
            Font = new Font("Arial", 16, FontStyle.Bold)
        };

        btnStartFront.Click += new EventHandler(this.BtnStartFront_Click);
        frontPanel.Controls.Add(btnStartFront);

        // Initialize the music player and start playing music in a loop
        InitializeMusicLoop();

        // Bring the front panel to the front
        frontPanel.BringToFront();
    }

        private void Form_Paint(object sender, PaintEventArgs e) 
        { 
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.DeepSkyBlue, Color.Black, 90F)) 
            { e.Graphics.FillRectangle(brush, this.ClientRectangle); } 
        }
        private void InitializeMusicLoop()
        {
            soundPlayer = new SoundPlayer(Properties.Resources.FrontMusic); 
           soundPlayer.PlayLooping(); 
        }
        private void PlayFrontMusic()
        {
            using (SoundPlayer player = new SoundPlayer(Properties.Resources.FrontMusic)) 
            {
                player.Play();
            }
        }
        private void StopMusicLoop()
        {
            soundPlayer.Stop(); 
        
        }
        private void BtnStartFront_Click(object sender, EventArgs e)
        {
            StopMusicLoop();
            // Hide the front panel and proceed with the quiz or next steps
            Panel frontPanel = (Panel)this.Controls["frontPanel"];
            if (frontPanel != null)
            {
                frontPanel.Visible = false;
            }
            ShowMenuForDay(selectedDay);
        }

        private void ShowFrontPanel()
        {
            InitializeFrontPanel();
            PlayFrontMusic();
        }

        private void InitializeScores()
        {
            foreach (var seat in seatingOrder)
            {
                string school = GetSchoolBySeatNumber(seat);
                scores[school] = 0;
            }
        }

        private void InitializeTimers()
        {
            // Initialize hearing question timer
            hearingQuestionTimer = new Timer();
            hearingQuestionTimer.Interval = 1000; // 1 second intervals
            hearingQuestionTimer.Tick += HearingTimer_Tick;

            // Initialize reading question timer
            readingQuestionTimer = new Timer();
            readingQuestionTimer.Interval = 1000; // 1 second intervals
            readingQuestionTimer.Tick += ReadingTimer_Tick;
            // Initialize the warning sound player
            warningSoundPlayer = new SoundPlayer(Properties.Resources.WarningSound);
        }

        private void InitializePanelDaySelection()
        {
          panelDaySelection=new Panel { 
              Dock = DockStyle.Fill,
              BackColor = Color.Transparent
          };
            this.Controls.Add((panelDaySelection));

                this.dayButtons = new Button[6];
                for (int i = 0; i < 6; i++)
                {
                    this.dayButtons[i] = new Button
                    {
                        Text = $"Day {i + 1}",
                        Location = new Point(10, 10 + (i * 50)),
                        Size = new Size(180, 40),
                        Tag = (i + 1) // Store day number in the Tag property
                    };
                    this.dayButtons[i].Click += new EventHandler(this.DayButton_Click);
                    this.panelDaySelection.Controls.Add(this.dayButtons[i]);
                }

                Button btnBackDaySelection = new Button
                {
                    Text = "Back",
                    Location = new Point(10, 320),
                    Size = new Size(180, 40)
                };
                btnBackDaySelection.Click += new EventHandler(this.btnBack_Click);
                this.panelDaySelection.Controls.Add(btnBackDaySelection);

                this.Controls.Add(this.panelDaySelection);
            

        }


        private void InitializeTimer()
        {
            questionTimer = new Timer(); 
            questionTimer.Interval = 1000; // 1 second
            questionTimer.Tick += Timer_Tick;
            // Initialize the warning sound player
            warningSoundPlayer = new SoundPlayer(Properties.Resources.WarningSound);
        }

        private void InitializeCountdownLabel()
        {
            lblTime = new Label
            {
                Location = new Point(10, 10), // Adjust the location as needed
                Size = new Size(200, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                Text = "Time remaining: 0" // Initial text
            };
            this.panelQuestions.Controls.Add(lblTime);
        }



        private void InitializePanelSchools()
        {
            panelSchools = new Panel {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelSchools);

            TextBox txtSchoolName = new TextBox { Location = new Point(10, 10), Size = new Size(380, 50) };
            Button btnAddSchool = new Button { Text = "Add School", Location = new Point(10, 40) };
            btnAddSchool.Click += new EventHandler(this.btnAddSchool_Click);
            listBoxSchools = new ListBox { Location = new Point(10, 70), Size = new Size(380, 200) };

            lblSchoolCount = new Label { Location = new Point(10, 280), AutoSize = true };
            Button btnBackSchools = new Button { Text = "Back", Location = new Point(100, 270), Size = new Size(80, 30) };
            btnBackSchools.Click += new EventHandler(this.BackButton_Click);

            Button btnResetSchools = new Button { Text = "Reset", Location = new Point(300, 270), Size = new Size(80, 30) };
            btnResetSchools.Click += new EventHandler(this.btnResetSchools_Click);

            Button btnDeleteSchool = new Button { Text = "Delete School", Location = new Point(190, 270), Size = new Size(100, 30) };
            btnDeleteSchool.Click += new EventHandler(this.btnDeleteSchool_Click);

            panelSchools.Controls.Add(txtSchoolName);
            panelSchools.Controls.Add(btnAddSchool);
            panelSchools.Controls.Add(listBoxSchools);
            panelSchools.Controls.Add(lblSchoolCount);
            panelSchools.Controls.Add(btnBackSchools);
            panelSchools.Controls.Add(btnResetSchools);
            panelSchools.Controls.Add(btnDeleteSchool);
            LoadSchools();
        }

        private void InitializePanelQuestions()
        {
            panelQuestions = new Panel { 
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelQuestions);

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

            lblQuestionCount = new Label { Location = new Point(150, 10), AutoSize = true };

            panelQuestions.Controls.Add(lblQuestion);
            panelQuestions.Controls.Add(pictureBoxQuestion);
            panelQuestions.Controls.Add(radioButtonA);
            panelQuestions.Controls.Add(radioButtonB);
            panelQuestions.Controls.Add(radioButtonC);
            panelQuestions.Controls.Add(radioButtonD);
            panelQuestions.Controls.Add(btnSubmitAnswer);
            panelQuestions.Controls.Add(btnBackQuestions);
            panelQuestions.Controls.Add(lblQuestionCount);
        }

        private void InitializePanelWheel()
        {
            panelWheel = new Panel { 
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelWheel);

            // Define a reasonable size for the wheel
            Size wheelSize = new Size(300, 300);

            // Calculate the position to center the wheel in the panel
            Point wheelPosition = new Point(
                (panelWheel.Width - wheelSize.Width) / 2,
                (panelWheel.Height - wheelSize.Height) / 2
            );

            wheelControl = new WheelControl(connection)
            {
                Location = wheelPosition,
                Size = wheelSize,
                Anchor = AnchorStyles.None // Ensure it stays centered
            };
            panelWheel.Controls.Add(wheelControl);

            // Add a button to spin the wheel
            Button btnSpin = new Button
            {
                Text = "Spin",
                Location = new Point(
                    (panelWheel.Width - 100) / 2,
                    wheelControl.Bottom + 10
                ),
                Size = new Size(100, 30)
            };
            btnSpin.Click += new EventHandler(this.btnSpin_Click);
            panelWheel.Controls.Add(btnSpin);

            // Add a back button
            Button btnBack = new Button
            {
                Text = "Back",
                Location = new Point(
                    (panelWheel.Width - 100) / 2,
                    wheelControl.Bottom + 50
                ),
                Size = new Size(100, 30)
            };
            btnBack.Click += new EventHandler(this.btnBack_Click);
            panelWheel.Controls.Add(btnBack);

            // Ensure the controls resize properly when the panel resizes
            panelWheel.Resize += (s, e) =>
            {
                wheelPosition = new Point(
                    (panelWheel.Width - wheelSize.Width) / 2,
                    (panelWheel.Height - wheelSize.Height) / 2
                );
                wheelControl.Location = wheelPosition;

                btnSpin.Location = new Point(
                    (panelWheel.Width - btnSpin.Width) / 2,
                    wheelControl.Bottom + 10
                );

                btnBack.Location = new Point(
                    (panelWheel.Width - btnBack.Width) / 2,
                    wheelControl.Bottom + 50
                );
            };
        }

        private void btnSpin_Click(object sender, EventArgs e)
        {
            wheelControl.Spin();
        }

        
        private void btnBack_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelMenu.Visible = true;
        }



        private void InitializePanelScores()
        {
            panelScores = new Panel
            {
                Size = new Size(400, 500), // Increase size for better visibility
                BackColor = Color.LightGray,
                Anchor = AnchorStyles.None // Ensure it stays centered
            };

            Label lblScores = new Label
            {
                Text = "Scores",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Font = new Font("Arial", 24, FontStyle.Bold),
                Height = 60, // Explicit height to ensure visibility
                Padding = new Padding(10) // Added padding to ensure visibility
            };
            panelScores.Controls.Add(lblScores);

            // Initialize listBoxScores
            listBoxScores = new ListBox
            {
                Dock = DockStyle.Fill, // Fill the remaining space
                Font = new Font("Arial", 12)
            };
            panelScores.Controls.Add(listBoxScores);

            // Add a back button
            Button btnBack = new Button
            {
                Text = "Back",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Arial", 12)
            };
            btnBack.Click += new EventHandler(this.btnBack_Click);
            panelScores.Controls.Add(btnBack);

            this.Controls.Add(panelScores);

            // Center the panel
            CenterPanel(panelScores);

            // Ensure the panel stays centered when the form is resized
            this.Resize += (s, e) => CenterPanel(panelScores);
        }

        private void CenterPanel(Control panel)
        {
            panel.Location = new Point(
                (this.ClientSize.Width - panel.Width) / 2,
                (this.ClientSize.Height - panel.Height) / 2
            );
        }




        private void InitializePanelQuestionSelection()
        {
            panelQuestionSelection = new Panel {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelQuestionSelection);


            LoadQuestionCount();
            LoadHearingQuestionCount();
            LoadReadingQuestionCount();

            int yOffset = 10;

            // Dynamically create buttons for each normal question
            for (int i = 1; i <= countQ; i++)
            {
                Button btnQuestion = new Button
                {
                    Text = $"Question {i}",
                    Name = $"btnQuestion{i}",
                    Location = new Point(10 + ((i - 1) % 2) * 200, yOffset + ((i - 1) / 2) * 50),
                    Size = new Size(180, 40)
                };
                btnQuestion.Click += new EventHandler(this.QuestionButton_Click);
                this.panelQuestionSelection.Controls.Add(btnQuestion);
            }

            // Offset for Hearing Questions section
            yOffset += ((countQ + 1) / 2) * 50 + 10;

            Label lblHearingQuestions = new Label
            {
                Text = "Hearing Questions",
                Location = new Point(10, yOffset),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            panelQuestionSelection.Controls.Add(lblHearingQuestions);

            yOffset += 30;

            // Dynamically create buttons for each hearing question
            for (int i = 1; i <= countHQ; i++)
            {
                Button btnHearingQuestion = new Button
                {
                    Text = $"Hearing Question {i}",
                    Name = $"btnHearingQuestion{i}",
                    Location = new Point(10 + ((i - 1) % 2) * 200, yOffset + ((i - 1) / 2) * 50),
                    Size = new Size(180, 40)
                };
                btnHearingQuestion.Click += new EventHandler(this.HearingQuestionButton_Click);
                this.panelQuestionSelection.Controls.Add(btnHearingQuestion);
            }

            // Offset for Reading Questions section
            yOffset += ((countQ + 1) / 2) * 50 + 10;

            Label lblReadingQuestions = new Label
            {
                Text = "Reading Questions",
                Location = new Point(10, yOffset),
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            panelQuestionSelection.Controls.Add(lblReadingQuestions);

            yOffset += 30;

            // Dynamically create buttons for each reading question
            for (int i = 1; i <= countRQ; i++)
            {
                Button btnReadingQuestion = new Button
                {
                    Text = $"Reading Question {i}",
                    Name = $"btnReadingQuestion{i}",
                    Location = new Point(10 + ((i - 1) % 2) * 200, yOffset + ((i - 1) / 2) * 50),
                    Size = new Size(180, 40)
                };
                btnReadingQuestion.Click += new EventHandler(this.ReadingQuestionButton_Click);
                this.panelQuestionSelection.Controls.Add(btnReadingQuestion);
            }

            Button btnBackQuestionSelection = new Button { Text = "Back", Location = new Point(100, yOffset + ((countQ + 1) / 2) * 50 + 30), Size = new Size(80, 30) };
            btnBackQuestionSelection.Click += new EventHandler(this.BackButton_Click);
            panelQuestionSelection.Controls.Add(btnBackQuestionSelection);

           
                if (isQuizMode)
                {
                    Button btnEndQuiz = new Button { Text = "End Quiz", Location = new Point(200, 300), Size = new Size(80, 30) };
                    btnEndQuiz.Click += new EventHandler(this.btnEndQuiz_Click);
                    panelQuestionSelection.Controls.Add(btnEndQuiz);
                }
            

        }
        private void HearingQuestionButton_Click(object sender, EventArgs e)
        {
            if (!isQuizMode)
            {
                MessageBox.Show("You are in view mode, question cannot be selected.");
                return;
            }

            string tableName = $"HearingQuestions{selectedDay}";

            Button clickedButton = (Button)sender;
            int questionNumber = int.Parse(clickedButton.Text.Replace("Hearing Question ", ""));
            chosenQ = questionNumber;

            // Retrieve the question details including the time limit
            string query = $"SELECT * FROM {tableName} WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);

                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Display the question using the existing method
                    DisplayHearingQuestion(reader);

                    // Start the timer with the time limit
                    int timeLimit = (int)reader["TimeLimit"];
                    StartHearingTimer(timeLimit);

                    // Mark the question as answered
                    MarkQuestionAsAnswered(questionNumber, clickedButton);
                }
                reader.Close();
                CloseConnection();
            }

            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }


        private void ReadingQuestionButton_Click(object sender, EventArgs e)
        {
            if (!isQuizMode)
            {
                MessageBox.Show("You are in view mode, question cannot be selected.");
                return;
            }

            string tableName = $"ReadingQuestions{selectedDay}";

            Button clickedButton = (Button)sender;
            int questionNumber = int.Parse(clickedButton.Text.Replace("Reading Question ", ""));
            chosenQ = questionNumber;

            // Retrieve the question details including the time limit
            string query = $"SELECT * FROM {tableName} WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);

                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Display the question using the existing method
                    DisplayReadingQuestion(reader);

                    // Start the timer with the time limit
                    int timeLimit = (int)reader["TimeLimit"];
                    StartReadingTimer(timeLimit);

                    // Mark the question as answered
                    MarkQuestionAsAnswered(questionNumber, clickedButton);
                }
                reader.Close();
                CloseConnection();
            }

            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }


        private void InitializePanelAddQuestions()
        {
            panelAddQuestion = new Panel { 
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelAddQuestion);

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
            this.panelAddQuestion.Controls.Add(rbOptionA);//1


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
            this.panelAddQuestion.Controls.Add(rbOptionB);//2


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
            this.panelAddQuestion.Controls.Add(rbOptionC);//3


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
            this.panelAddQuestion.Controls.Add(rbOptionD);//4


            // Save Button
            Button btnSaveQuestion = new Button { Text = "Save Question", Location = new Point(150, 490) };
            btnSaveQuestion.Click += new EventHandler(this.btnSaveQuestion_Click);

            // Back Button
            Button btnBackAddQuestions = new Button { Text = "Back", Location = new Point(300, 470), Size = new Size(80, 30) };
            btnBackAddQuestions.Click += new EventHandler(this.BackButton_Click);


                // Initialize ListBox for questions
                ListBox listBoxQuestions = new ListBox();
                listBoxQuestions.Name = "listBoxQuestions";
                listBoxQuestions.Location = new Point(450, 10);
                listBoxQuestions.Size = new Size(260, 200);
                

                // Initialize Delete Button
                Button buttonDelete = new Button();
                buttonDelete.Name = "buttonDelete";
                buttonDelete.Text = "Delete";
                buttonDelete.Location = new Point(450, 220);
                buttonDelete.Click += new EventHandler(ButtonDelete_Click);
                

                // Initialize Reset Button
                Button buttonReset = new Button();
                buttonReset.Name = "buttonReset";
                buttonReset.Text = "Reset";
                buttonReset.Location = new Point(500, 220);
                buttonReset.Click += new EventHandler(ButtonReset_Click);
                
            


            // Add Controls to Panel
            this.panelAddQuestion.Controls.Add(lblQuestionText);//5
            this.panelAddQuestion.Controls.Add(txtQuestionText);//6
            this.panelAddQuestion.Controls.Add(lblQuestionImage);//7
            this.panelAddQuestion.Controls.Add(txtQuestionImage);//8
            this.panelAddQuestion.Controls.Add(btnBrowseImage);//9
            this.panelAddQuestion.Controls.Add(lblQuestionAudio);//10
            this.panelAddQuestion.Controls.Add(txtQuestionAudio);//11
            this.panelAddQuestion.Controls.Add(btnBrowseAudio);//12
            this.panelAddQuestion.Controls.Add(lblOptionA);//13
            this.panelAddQuestion.Controls.Add(txtOptionAText);//14
            this.panelAddQuestion.Controls.Add(txtOptionAImage);//15
            this.panelAddQuestion.Controls.Add(btnBrowseOptionAImage);//16
            this.panelAddQuestion.Controls.Add(txtOptionAAudio);//17
            this.panelAddQuestion.Controls.Add(btnBrowseOptionAAudio);//18
            this.panelAddQuestion.Controls.Add(lblOptionB);//19
            this.panelAddQuestion.Controls.Add(txtOptionBText);//20
            this.panelAddQuestion.Controls.Add(txtOptionBImage);//21
            this.panelAddQuestion.Controls.Add(btnBrowseOptionBImage);//22
            this.panelAddQuestion.Controls.Add(txtOptionBAudio);//23
            this.panelAddQuestion.Controls.Add(btnBrowseOptionBAudio);//24
            this.panelAddQuestion.Controls.Add(lblOptionC);//25
            this.panelAddQuestion.Controls.Add(txtOptionCText);//26
            this.panelAddQuestion.Controls.Add(txtOptionCImage);//27
            this.panelAddQuestion.Controls.Add(btnBrowseOptionCImage);//28
            this.panelAddQuestion.Controls.Add(txtOptionCAudio);//29
            this.panelAddQuestion.Controls.Add(btnBrowseOptionCAudio);//30
            this.panelAddQuestion.Controls.Add(lblOptionD);//31
            this.panelAddQuestion.Controls.Add(txtOptionDText);//32
            this.panelAddQuestion.Controls.Add(txtOptionDImage);//33
            this.panelAddQuestion.Controls.Add(btnBrowseOptionDImage);//34
            this.panelAddQuestion.Controls.Add(txtOptionDAudio);//35
            this.panelAddQuestion.Controls.Add(btnBrowseOptionDAudio);//36
            this.panelAddQuestion.Controls.Add(btnSaveQuestion);//37
            this.panelAddQuestion.Controls.Add(btnBackAddQuestions);//38
            this.panelAddQuestion.Controls.Add(listBoxQuestions);//39
            this.panelAddQuestion.Controls.Add(buttonDelete);//40
            this.panelAddQuestion.Controls.Add(buttonReset);//41

            // Time Limit
            Label lblTimeLimit = new Label { Text = "Time Limit (seconds):", Location = new Point(10, 460) };
            TextBox txtTimeLimit = new TextBox { Location = new Point(180, 460), Width = 120 };
            panelAddQuestion.Controls.Add(lblTimeLimit);//42
            panelAddQuestion.Controls.Add(txtTimeLimit);//43


            // Load existing questions into the ListBox
            LoadQuestionsIntoListBox();
        }
        private void HideAllPanels()
        {
            this.panelMenu.Visible = false;
            this.panelSchools.Visible = false;
            this.panelQuestions.Visible = false;
            this.panelWheel.Visible = false;
            this.panelScores.Visible = false;
            this.panelQuestionSelection.Visible = false;
            this.panelAddQuestion.Visible = false;
            this.panelStart.Visible = false;
            this.panelDaySelection.Visible = false;
            this.panelAddHearingQuestion.Visible = false;
            this.panelAddReadingQuestion.Visible = false;   
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.panelStart.Visible = false;
            HideAllPanels();
            this.panelDaySelection.Visible = true;
        }
        private void DayButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                selectedDay = (int)clickedButton.Tag;
                ShowFrontPanel();
                
               
            }
            else
            {
                MessageBox.Show("Error: Clicked button is null.");
            }
        }


        private void ShowMenuForDay(int dayIndex)
        {
            selectedDay = dayIndex;
            HideAllPanels();
            this.panelMenu.Visible = true;
            LoadQuestionCount();
            LoadQuestionsIntoListBox();
           LoadHearingQuestionCount();
            LoadReadingQuestionCount();
            LoadHearingQuestionsIntoListBox();
            LoadReadingQuestionsIntoListBox();
            LoadSchoolCount();
            LoadSchools();
            LoadSchoolsIntoWheel();
            LoadScores();
            InitializePanelQuestionSelection();
        }




        private void btnSchools_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelSchools.Visible = true;
            LoadSchoolCount();
            LoadSchools();
        }

        private void btnQuestions_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelQuestionSelection.Visible = true;
            LoadQuestionCount();
            
        }

        private void btnWheel_Click(object sender, EventArgs e)
        {
            // Reset the SeatingArrangements table
            //  ResetSeatingArrangementsTable();
            LoadSchoolsIntoWheel();
            // Hide all other panels
            HideAllPanels();
            
            wheelControl.SelectedDay = this.selectedDay;
            InitializePanelWheel();

            // Show the wheel panel
            this.panelWheel.Visible = true;
        }

        private void ResetSeatingArrangementsTable()
        {
           
            string tableName = $"SeatingArrangements{selectedDay}";

            string dropTableQuery = $@"
    IF OBJECT_ID('dbo.{tableName}', 'U') IS NOT NULL
    DROP TABLE dbo.{tableName};
";

            string createTableQuery = $@"
    CREATE TABLE {tableName} (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        SeatNumber INT,
        SchoolID INT,
        CONSTRAINT FK_{tableName}_Schools FOREIGN KEY (SchoolID) REFERENCES Schools{selectedDay}(ID)
    );
";

            using (SqlCommand command = new SqlCommand(dropTableQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            using (SqlCommand command = new SqlCommand(createTableQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            MessageBox.Show($"Seating arrangements for Day {selectedDay} have been reset.");
        }


        private void btnScores_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelScores.Visible = true;
            LoadScores();
        }

        private void btnAddQuestions_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelAddQuestion.Visible = true;
        }

        private void LoadSchoolsIntoWheel()
        {
            
            string tableName = $"Schools{selectedDay}";
            string query = $"SELECT Name FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    wheelControl.AddItem(reader["Name"].ToString());
                }
                reader.Close();
                CloseConnection();
            }
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            this.panelMenu.Visible = true;
        }



        private void LoadQuestionsIntoListBox()
        {
            string tableName = $"Questions{selectedDay}";

            ListBox listBoxQuestions = (ListBox)this.panelAddQuestion.Controls["listBoxQuestions"];
            if (listBoxQuestions != null)
            {
                listBoxQuestions.Items.Clear();
                string query = $"SELECT ID FROM {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listBoxQuestions.Items.Add(reader["ID"].ToString());
                    }
                    reader.Close();
                    CloseConnection();
                }
            }
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

        private void btnResetSchools_Click(object sender, EventArgs e)
        {
           
            string tableName = $"Schools{selectedDay}";
            string seatingTableName = $"SeatingArrangements{selectedDay}";

            using (SqlConnection connection = new SqlConnection("Data Source=MALAKS-LAPTOP\\SQLEXPRESS;Initial Catalog=AwaeelMusic;Integrated Security=True;Encrypt=False;"))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Delete related records from SeatingArrangements table
                    string deleteSeatingQuery = $"DELETE FROM {seatingTableName}";
                    using (SqlCommand command = new SqlCommand(deleteSeatingQuery, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Delete records from Schools table
                    string deleteSchoolsQuery = $"DELETE FROM {tableName}";
                    using (SqlCommand command = new SqlCommand(deleteSchoolsQuery, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Reset identity seed value for Schools table
                    string resetIdQuery = $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)";
                    using (SqlCommand command = new SqlCommand(resetIdQuery, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show("Schools have been reset successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error resetting schools: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            LoadSchools();
            LoadSchoolCount();
        }



        private void LoadQuestionCount()
        {
            string tableName = $"Questions{selectedDay}";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int questionCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblQuestionCount.Text = $"Number of Questions: {questionCount}";
                countQ = questionCount;
            }
        }



        private void ButtonDelete_Click(object sender, EventArgs e)
        {
         
            ListBox listBoxQuestions = (ListBox)this.panelAddQuestion.Controls["listBoxQuestions"];
            if (listBoxQuestions != null && listBoxQuestions.SelectedItem != null)
            {
                // Get the selected question
                string selectedQuestion = listBoxQuestions.SelectedItem.ToString();
                string tableName = $"Questions{selectedDay}";

                // Remove from the ListBox
                listBoxQuestions.Items.Remove(listBoxQuestions.SelectedItem);

                // Delete from the database
                string deleteQuery = $"DELETE FROM {tableName} WHERE ID = @ID";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", selectedQuestion);
                    OpenConnection();
                    command.ExecuteNonQuery();
                    CloseConnection();
                }

                // Resequence IDs
                ResequenceQuestionIDs();
                LoadQuestionsIntoListBox();

                MessageBox.Show("Question deleted successfully.");
                LoadQuestionCount();
            }
            else
            {
                MessageBox.Show("Please select a question to delete.");
            }
        }

        private void ResequenceQuestionIDs()
        {
          
            string tableName = $"Questions{selectedDay}";
            string tempTableName = $"{tableName}_Temp";

            string createTempTableQuery = $@"
    CREATE TABLE {tempTableName} (
        ID INT PRIMARY KEY,
        QuestionText NVARCHAR(MAX),
        QuestionImage VARBINARY(MAX),
        QuestionAudio VARBINARY(MAX),
        OptionA NVARCHAR(MAX),
        OptionAImage VARBINARY(MAX),
        OptionAAudio VARBINARY(MAX),
        OptionB NVARCHAR(MAX),
        OptionBImage VARBINARY(MAX),
        OptionBAudio VARBINARY(MAX),
        OptionC NVARCHAR(MAX),
        OptionCImage VARBINARY(MAX),
        OptionCAudio VARBINARY(MAX),
        OptionD NVARCHAR(MAX),
        OptionDImage VARBINARY(MAX),
        OptionDAudio VARBINARY(MAX),
        CorrectAnswer NVARCHAR(MAX),
        IsAnswered BIT
    );
    
    INSERT INTO {tempTableName} (ID, QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer, IsAnswered)
    SELECT ROW_NUMBER() OVER (ORDER BY ID) AS NewID, QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer, IsAnswered
    FROM {tableName};
    
    DROP TABLE {tableName};
    
    EXEC sp_rename '{tempTableName}', '{tableName}';
";

            using (SqlCommand command = new SqlCommand(createTempTableQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
           
            ListBox listBoxQuestions = (ListBox)this.panelAddQuestion.Controls["listBoxQuestions"];
            if (listBoxQuestions != null)
            {
                listBoxQuestions.Items.Clear();
            }

            string tableName = $"Questions{selectedDay}";
            string resetQuery = $"DELETE FROM {tableName}";
            using (SqlCommand command = new SqlCommand(resetQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            string reseedQuery = $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)";
            using (SqlCommand command = new SqlCommand(reseedQuery, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            MessageBox.Show($"All questions for Day {selectedDay} have been reset.");
            LoadQuestionCount();
        }





        private void btnSaveQuestion_Click(object sender, EventArgs e)
        {
            
            string tableName = $"Questions{selectedDay}";

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

            // Time Limit
            int timeLimit = 0;
            if (!int.TryParse(((TextBox)this.panelAddQuestion.Controls[42]).Text, out timeLimit))
            {
                MessageBox.Show("Please enter a valid number for the time limit.");
                return;
            }

            // Determine Correct Answer
            string correctAnswer = string.Empty;
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionA"]).Checked) correctAnswer = "A";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionB"]).Checked) correctAnswer = "B";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionC"]).Checked) correctAnswer = "C";
            if (((RadioButton)this.panelAddQuestion.Controls["rbOptionD"]).Checked) correctAnswer = "D";

            string query = $@"
    INSERT INTO {tableName} (QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer, TimeLimit) 
    VALUES (@QuestionText, CONVERT(VARBINARY(MAX), @QuestionImage), CONVERT(VARBINARY(MAX), @QuestionAudio), 
    @OptionAText, CONVERT(VARBINARY(MAX), @OptionAImage), CONVERT(VARBINARY(MAX), @OptionAAudio), 
    @OptionBText, CONVERT(VARBINARY(MAX), @OptionBImage), CONVERT(VARBINARY(MAX), @OptionBAudio), 
    @OptionCText, CONVERT(VARBINARY(MAX), @OptionCImage), CONVERT(VARBINARY(MAX), @OptionCAudio), 
    @OptionDText, CONVERT(VARBINARY(MAX), @OptionDImage), CONVERT(VARBINARY(MAX), @OptionDAudio), 
    @CorrectAnswer, @TimeLimit)";

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
                command.Parameters.AddWithValue("@TimeLimit", (object)timeLimit ?? DBNull.Value);

                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            LoadQuestionsIntoListBox();

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
                int day = selectedDay; // Use the selected day to determine which tables to use

                using (SqlConnection connection = new SqlConnection("Data Source=MALAKS-LAPTOP\\SQLEXPRESS;Initial Catalog=AwaeelMusic;Integrated Security=True;Encrypt=False;"))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        string tableName = $"Schools{day}";
                        string seatingTableName = $"SeatingArrangements{day}";
                        string scoresTableName = $"Scores{day}";

                        // Get School ID for the current selected day
                        int schoolID = GetSchoolID(selectedSchool, connection, transaction);

                        // Drop foreign key constraints
                        DropForeignKeyConstraintsSeating(seatingTableName, connection, transaction, day);
                        DropForeignKeyConstraintsScores(scoresTableName, connection, transaction, day);

                        // Delete related records from SeatingArrangements table
                        string deleteSeatingQuery = $"DELETE FROM {seatingTableName} WHERE SchoolID = @SchoolID";
                        using (SqlCommand command = new SqlCommand(deleteSeatingQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SchoolID", schoolID);
                            command.ExecuteNonQuery();
                        }

                        // Delete related records from Scores table
                        string deleteScoresQuery = $"DELETE FROM {scoresTableName} WHERE SchoolID = @SchoolID";
                        using (SqlCommand command = new SqlCommand(deleteScoresQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SchoolID", schoolID);
                            command.ExecuteNonQuery();
                        }

                        // Delete records from Schools table
                        string deleteSchoolsQuery = $"DELETE FROM {tableName} WHERE ID = @SchoolID";
                        using (SqlCommand command = new SqlCommand(deleteSchoolsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SchoolID", schoolID);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show($"School deleted successfully from {tableName}.");
                            }
                            else
                            {
                                MessageBox.Show($"Failed to delete the school from {tableName}. Please try again.");
                            }
                        }

                        // Resequence School IDs
                        ResequenceSchoolIDs(connection, transaction);

                        // Recreate foreign key constraints
                        RecreateForeignKeyConstraintsSeating(seatingTableName, connection, transaction, day);
                        RecreateForeignKeyConstraintsScores(scoresTableName, connection, transaction, day);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error deleting school: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
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


        private void DropForeignKeyConstraintsSeating(string seatingTableName, SqlConnection connection, SqlTransaction transaction, int day)
        {
            string constraintName = $"FK_SeatingArrangements{day}_Schools";
            string dropConstraintQuery = $@"
    ALTER TABLE {seatingTableName} 
    DROP CONSTRAINT {constraintName}";

            using (SqlCommand command = new SqlCommand(dropConstraintQuery, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }



        private void RecreateForeignKeyConstraintsSeating(string seatingTableName, SqlConnection connection, SqlTransaction transaction, int day)
        {
            string tableName = $"Schools{day}";
            string constraintName = $"FK_SeatingArrangements{day}_Schools";
            string createConstraintQuery = $@"
    ALTER TABLE {seatingTableName} 
    ADD CONSTRAINT {constraintName} 
    FOREIGN KEY (SchoolID) REFERENCES {tableName}(ID)";

            using (SqlCommand command = new SqlCommand(createConstraintQuery, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private void DropForeignKeyConstraintsScores(string scoresTableName, SqlConnection connection, SqlTransaction transaction, int day)
        {
            string constraintName = $"FK_Scores_Schools{day}";
            string dropConstraintQuery = $@"
    ALTER TABLE {scoresTableName} 
    DROP CONSTRAINT {constraintName}";

            using (SqlCommand command = new SqlCommand(dropConstraintQuery, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }


        private void RecreateForeignKeyConstraintsScores(string scoresTableName, SqlConnection connection, SqlTransaction transaction, int day)
        {
            string tableName = $"Schools{day}";
            string constraintName = $"FK_Scores_Schools{day}";
            string createConstraintQuery = $@"
    ALTER TABLE {scoresTableName} 
    ADD CONSTRAINT {constraintName} 
    FOREIGN KEY (SchoolID) REFERENCES {tableName}(ID)";

            using (SqlCommand command = new SqlCommand(createConstraintQuery, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }




        private void ResequenceSchoolIDs(SqlConnection connection, SqlTransaction transaction)
        {
            string tableName = $"Schools{selectedDay}";
            string tempTableName = $"{tableName}_Temp";

            string createTempTableQuery = $@"
    CREATE TABLE {tempTableName} (
        ID INT PRIMARY KEY,
        Name NVARCHAR(MAX)
    );

    INSERT INTO {tempTableName} (ID, Name)
    SELECT ROW_NUMBER() OVER (ORDER BY ID) AS NewID, Name
    FROM {tableName};

    DROP TABLE {tableName};

    EXEC sp_rename '{tempTableName}', '{tableName}';
    ";

            using (SqlCommand command = new SqlCommand(createTempTableQuery, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }



        private void QuestionButton_Click(object sender, EventArgs e)
        {
            if (!isQuizMode)
            {
                MessageBox.Show("You are in view mode, question cannot be selected.");
                return;
            }

            string tableName = $"Questions{selectedDay}";

            Button clickedButton = (Button)sender;
            int questionNumber = int.Parse(clickedButton.Text.Replace("Question ", ""));
            chosenQ = questionNumber;

            // Retrieve the question details including the time limit
            string query = $"SELECT * FROM {tableName} WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);

                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Display the question using the existing method
                    DisplayQuestion(reader);

                    // Start the timer with the time limit
                    int timeLimit = (int)reader["TimeLimit"];
                    StartTimer(timeLimit);

                    // Mark the question as answered
                    MarkQuestionAsAnswered(questionNumber, clickedButton);
                }
                reader.Close();
                CloseConnection();
            }

            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }

        private void StartTimer(int timeLimit)
        {
            timeRemaining = timeLimit; // Set the time limit for the question
            questionTimer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            timeRemaining--;
            if (timeRemaining == 10) { warningSoundPlayer.Play(); }
            if (timeRemaining <= 0)
            {
                questionTimer.Stop();
                warningSoundPlayer.Stop();
                MessageBox.Show("Time's up!");
                // Implement logic for what happens when time is up
            }
            else
            {
                lblTime.Text = $"Time remaining: {timeRemaining} seconds";
            }
        }




        private void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            string selectedAnswer = string.Empty;
            if (radioButtonA.Checked) selectedAnswer = "A";
            if (radioButtonB.Checked) selectedAnswer = "B";
            if (radioButtonC.Checked) selectedAnswer = "C";
            if (radioButtonD.Checked) selectedAnswer = "D";

            int questionNumber = chosenQ;
            int schoolID = turn;
            turn++;
            if (turn > countSchools) turn = 1;

            bool isCorrect = false;

            // Determine the type of question and check the answer accordingly
            switch (currentQuestionType)
            {
                case QuestionType.Hearing:
                    isCorrect = CheckHearingAnswer(selectedAnswer, questionNumber);
                    break;
                case QuestionType.Reading:
                    isCorrect = CheckReadingAnswer(selectedAnswer, questionNumber);
                    break;
                case QuestionType.Normal:
                    isCorrect = CheckNormalAnswer(selectedAnswer, questionNumber);
                    break;
            }

            if (isCorrect)
            {
                UpdateScores(schoolID, 1);
            }

            // Mark the question as answered based on its type
            switch (currentQuestionType)
            {
                case QuestionType.Hearing:
                    MarkHearingQuestionAsAnswered(questionNumber);
                    break;
                case QuestionType.Reading:
                    MarkReadingQuestionAsAnswered(questionNumber);
                    break;
                case QuestionType.Normal:
                    MarkNormalQuestionAsAnswered(questionNumber);
                    break;
            }

            DisplayNextQuestion();
        }
        public enum QuestionType
        {
            Hearing,
            Reading,
            Normal
        }
        private bool CheckNormalAnswer(string selectedAnswer, int questionNumber)
        {
            string tableName = $"Questions{selectedDay}";

            string query = $"SELECT CorrectAnswer FROM {tableName} WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                string correctAnswer = command.ExecuteScalar().ToString();
                CloseConnection();

                return selectedAnswer == correctAnswer;
            }
        }
        private void MarkQuestionAsAnswered(int questionNumber)
        {
            switch (currentQuestionType)
            {
                case QuestionType.Hearing:
                    MarkHearingQuestionAsAnswered(questionNumber);
                    break;
                case QuestionType.Reading:
                    MarkReadingQuestionAsAnswered(questionNumber);
                    break;
                case QuestionType.Normal:
                    MarkNormalQuestionAsAnswered(questionNumber);
                    break;
                default:
                    throw new Exception("Unknown question type.");
            }
        }

        private void MarkHearingQuestionAsAnswered(int questionNumber)
        {
            string query = $"UPDATE HearingQuestions{selectedDay} SET IsAnswered = 1 WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void MarkReadingQuestionAsAnswered(int questionNumber)
        {
            string query = $"UPDATE ReadingQuestions{selectedDay} SET IsAnswered = 1 WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void MarkNormalQuestionAsAnswered(int questionNumber)
        {
            string query = $"UPDATE Questions{selectedDay} SET IsAnswered = 1 WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }


        private void LoadSchoolCount()
        {
            string tableName = $"Schools{selectedDay}";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int schoolCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblSchoolCount.Text = $"Number of Schools: {schoolCount}";
                countSchools = schoolCount;
            }
        }



        private void MarkQuestionAsAnswered(int questionIndex, Button questionButton)
        {
            string tableName;

            if (questionButton.Name.StartsWith("btnQuestion"))
            {
                tableName = "Questions";
            }
            else if (questionButton.Name.StartsWith("btnHearingQuestion"))
            {
                tableName = "HearingQuestions";
            }
            else if (questionButton.Name.StartsWith("btnReadingQuestion"))
            {
                tableName = "ReadingQuestions";
            }
            else
            {
                throw new Exception("Unknown question type.");
            }

            string query = $"UPDATE {tableName}{selectedDay} SET IsAnswered = 1 WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionIndex);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }

            // Visually mark the question as answered
            questionButton.Enabled = false;
            questionButton.BackColor = Color.Gray;
        }



        private void DisplayNextQuestion()
        {
            this.panelQuestions.Visible = false;
            this.panelQuestionSelection.Visible = true;
        }

        private void btnAddSchool_Click(object sender, EventArgs e)
        {
           
            // Retrieve the school name from the input
            string schoolName = ((TextBox)this.panelSchools.Controls[0]).Text; 
            int day = selectedDay; // Use the selected day to determine which table to use
            string tableName = $"Schools{day}";

            // Ensure the connection is open before executing the query
            string query = $@"
    INSERT INTO {tableName} (Name) 
    VALUES (@SchoolName)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Use parameters to avoid SQL injection
                command.Parameters.AddWithValue("@SchoolName", (object)schoolName ?? DBNull.Value);

                // Open the connection
                OpenConnection();
                try
                {
                    // Execute the query to insert the new school
                    command.ExecuteNonQuery();

                    // Display a success message
                    MessageBox.Show("School added successfully.");
                }
                catch (Exception ex)
                {
                    // Display any errors that occur
                    MessageBox.Show($"Error adding school: {ex.Message}");
                }
                finally
                {
                    // Ensure the connection is closed
                    CloseConnection();
                }
            }

            // Reload the schools into the list box or panel
            LoadSchools();
            LoadSchoolCount();
        }



        private void LoadSchools()
        {
            string tableName = $"Schools{selectedDay}";

            string query = $"SELECT Name FROM {tableName}";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            listBoxSchools.DataSource = table;
            listBoxSchools.DisplayMember = "Name";
        }


        private void ArrangeSeating(string selectedSchool)
        {
            panelSeating.Controls.Clear();
            int numberOfSeats = 10;
            string connectionString = "Data Source=MALAKS-LAPTOP\\SQLEXPRESS;Initial Catalog=AwaeelMusic;Integrated Security=True;Encrypt=False;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
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

                    int schoolID = GetSchoolID(selectedSchool, connection, transaction); // Updated method call
                    SaveSeatingArrangement(schoolID, connection, transaction); // Ensure SaveSeatingArrangement handles transaction

                    transaction.Commit();
                    MessageBox.Show($"Seating arranged for {selectedSchool}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error arranging seating: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }



        private int GetSchoolID(string schoolName, SqlConnection connection, SqlTransaction transaction)
        {
           
            int schoolID = 0;
            string tableName = $"Schools{selectedDay}";
            string query = $"SELECT ID FROM {tableName} WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Name", schoolName);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    schoolID = (int)reader["ID"];
                }
                reader.Close();
            }
            return schoolID;
        }





        private void SaveSeatingArrangement(int schoolID, SqlConnection connection, SqlTransaction transaction)
        {
            
            string seatingTableName = $"SeatingArrangements{selectedDay}";
            string insertQuery = $"INSERT INTO {seatingTableName} (SeatNumber, SchoolID) VALUES (@SeatNumber, @SchoolID)";

            using (SqlCommand command = new SqlCommand(insertQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@SeatNumber", seatNumber);
                command.Parameters.AddWithValue("@SchoolID", schoolID);
                command.ExecuteNonQuery();
            }

            seatNumber++;
        }





        private void DisplayQuestion(SqlDataReader reader)
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


        private bool CheckAnswer(string selectedAnswer, int questionNumber)
        {
            switch (currentQuestionType)
            {
                case QuestionType.Hearing:
                    return CheckHearingAnswer(selectedAnswer, questionNumber);
                case QuestionType.Reading:
                    return CheckReadingAnswer(selectedAnswer, questionNumber);
                case QuestionType.Normal:
                    return CheckNormalAnswer(selectedAnswer, questionNumber);
                default:
                    throw new Exception("Unknown question type.");
            }
        }





        private void UpdateScores(int schoolID, int scoreToAdd)
        {
            string tableName = $"Scores{selectedDay}";

            string query = $"UPDATE {tableName} SET Score = Score + @ScoreToAdd WHERE SchoolID = @SchoolID";

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
            
            listBoxScores.Items.Clear();
            string scoresTableName = $"Scores{selectedDay}";
            string schoolsTableName = $"Schools{selectedDay}";

            string query = $@"
    SELECT s.Score, sch.Name 
    FROM {scoresTableName} s
    JOIN {schoolsTableName} sch ON s.SchoolID = sch.ID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Display school name and score
                    string item = $"{reader["Name"]}: {reader["Score"]}";
                    listBoxScores.Items.Add(item);
                }
                reader.Close();
                CloseConnection();
            }
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
        private void btnBrowseHearingQuestionImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtQuestionImage = (TextBox)this.panelAddHearingQuestion.Controls["txtQuestionImage"];
                if (txtQuestionImage != null)
                {
                    txtQuestionImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Hearing Question Image not found.");
                }
            }
        }

        private void btnBrowseHearingQuestionAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtQuestionAudio = (TextBox)this.panelAddHearingQuestion.Controls["txtQuestionAudio"];
                if (txtQuestionAudio != null)
                {
                    txtQuestionAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Hearing Question Audio not found.");
                }
            }
        }

        private void btnBrowseHearingOptionAImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAImage = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionAImage"];
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

        private void btnBrowseHearingOptionAAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAAudio = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionAAudio"];
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

        private void btnBrowseHearingOptionBImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBImage = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionBImage"];
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

        private void btnBrowseHearingOptionBAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBAudio = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionBAudio"];
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

        private void btnBrowseHearingOptionCImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCImage = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionCImage"];
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

        private void btnBrowseHearingOptionCAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCAudio = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionCAudio"];
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

        private void btnBrowseHearingOptionDImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDImage = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionDImage"];
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

        private void btnBrowseHearingOptionDAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDAudio = (TextBox)this.panelAddHearingQuestion.Controls["txtOptionDAudio"];
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

        private void btnBrowseReadingQuestionImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtQuestionImage = (TextBox)this.panelAddReadingQuestion.Controls["txtQuestionImage"];
                if (txtQuestionImage != null)
                {
                    txtQuestionImage.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Reading Question Image not found.");
                }
            }
        }

        private void btnBrowseReadingQuestionAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtQuestionAudio = (TextBox)this.panelAddReadingQuestion.Controls["txtQuestionAudio"];
                if (txtQuestionAudio != null)
                {
                    txtQuestionAudio.Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Textbox for Reading Question Audio not found.");
                }
            }
        }

        private void btnBrowseReadingOptionAImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAImage = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionAImage"];
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

        private void btnBrowseReadingOptionAAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option A"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionAAudio = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionAAudio"];
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

        private void btnBrowseReadingOptionBImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBImage = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionBImage"];
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

        private void btnBrowseReadingOptionBAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option B"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionBAudio = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionBAudio"];
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

        private void btnBrowseReadingOptionCImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCImage = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionCImage"];
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

        private void btnBrowseReadingOptionCAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option C"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionCAudio = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionCAudio"];
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

        private void btnBrowseReadingOptionDImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Select an Image for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDImage = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionDImage"];
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

        private void btnBrowseReadingOptionDAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.wav;*.mp3",
                Title = "Select an Audio File for Option D"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox txtOptionDAudio = (TextBox)this.panelAddReadingQuestion.Controls["txtOptionDAudio"];
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
        private void btnResetHearingQuestions_Click(object sender, EventArgs e)
        {
            string tableName = $"HearingQuestions{selectedDay}";
            string query = $"DELETE FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("All hearing questions have been deleted.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting hearing questions: {ex.Message}");
                }
                finally
                {
                    CloseConnection();
                }
            }

            LoadHearingQuestionsIntoListBox();
            LoadHearingQuestionCount();
        }

        private void btnResetReadingQuestions_Click(object sender, EventArgs e)
        {
            string tableName = $"ReadingQuestions{selectedDay}";
            string query = $"DELETE FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("All reading questions have been deleted.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting reading questions: {ex.Message}");
                }
                finally
                {
                    CloseConnection();
                }
            }

            LoadReadingQuestionsIntoListBox();
            LoadReadingQuestionCount();
        }
        private void InitializePanelAddHearingQuestions()
        {
            panelAddHearingQuestion = new Panel { 
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelAddHearingQuestion);

            // Question Text
            Label lblQuestionText = new Label { Text = "Question Text", Location = new Point(10, 10) };
            TextBox txtQuestionText = new TextBox { Location = new Point(150, 10), Width = 200 };

            // Question Image
            Label lblQuestionImage = new Label { Text = "Question Image", Location = new Point(10, 40) };
            TextBox txtQuestionImage = new TextBox { Location = new Point(150, 40), Width = 200 };
            Button btnBrowseImage = new Button { Text = "Browse", Location = new Point(360, 40) };
            btnBrowseImage.Click += new EventHandler(this.btnBrowseHearingQuestionImage_Click);

            // Question Audio
            Label lblQuestionAudio = new Label { Text = "Question Audio", Location = new Point(10, 70) };
            TextBox txtQuestionAudio = new TextBox { Location = new Point(150, 70), Width = 200 };
            Button btnBrowseAudio = new Button { Text = "Browse", Location = new Point(360, 70) };
            btnBrowseAudio.Click += new EventHandler(this.btnBrowseHearingQuestionAudio_Click);

            // Option A
            Label lblOptionA = new Label { Text = "Option A", Location = new Point(10, 100) };
            TextBox txtOptionAText = new TextBox { Location = new Point(150, 100), Width = 200 };
            TextBox txtOptionAImage = new TextBox { Location = new Point(150, 130), Width = 200 };
            Button btnBrowseOptionAImage = new Button { Text = "Browse", Location = new Point(360, 130) };
            btnBrowseOptionAImage.Click += new EventHandler(this.btnBrowseHearingOptionAImage_Click);
            TextBox txtOptionAAudio = new TextBox { Location = new Point(150, 160), Width = 200 };
            Button btnBrowseOptionAAudio = new Button { Text = "Browse", Location = new Point(360, 160) };
            btnBrowseOptionAAudio.Click += new EventHandler(this.btnBrowseHearingOptionAAudio_Click);
            RadioButton rbOptionA = new RadioButton { Text = "Correct", Location = new Point(450, 100), Name = "rbOptionAHearing" };
            rbOptionA.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
          

            // Option B
            Label lblOptionB = new Label { Text = "Option B", Location = new Point(10, 190) };
            TextBox txtOptionBText = new TextBox { Location = new Point(150, 190), Width = 200 };
            TextBox txtOptionBImage = new TextBox { Location = new Point(150, 220), Width = 200 };
            Button btnBrowseOptionBImage = new Button { Text = "Browse", Location = new Point(360, 220) };
            btnBrowseOptionBImage.Click += new EventHandler(this.btnBrowseHearingOptionBImage_Click);
            TextBox txtOptionBAudio = new TextBox { Location = new Point(150, 250), Width = 200 };
            Button btnBrowseOptionBAudio = new Button { Text = "Browse", Location = new Point(360, 250) };
            btnBrowseOptionBAudio.Click += new EventHandler(this.btnBrowseHearingOptionBAudio_Click);
            RadioButton rbOptionB = new RadioButton { Text = "Correct", Location = new Point(450, 190), Name = "rbOptionBHearing" };
            rbOptionB.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
           

            // Option C
            Label lblOptionC = new Label { Text = "Option C", Location = new Point(10, 280) };
            TextBox txtOptionCText = new TextBox { Location = new Point(150, 280), Width = 200 };
            TextBox txtOptionCImage = new TextBox { Location = new Point(150, 310), Width = 200 };
            Button btnBrowseOptionCImage = new Button { Text = "Browse", Location = new Point(360, 310) };
            btnBrowseOptionCImage.Click += new EventHandler(this.btnBrowseHearingOptionCImage_Click);
            TextBox txtOptionCAudio = new TextBox { Location = new Point(150, 340), Width = 200 };
            Button btnBrowseOptionCAudio = new Button { Text = "Browse", Location = new Point(360, 340) };
            btnBrowseOptionCAudio.Click += new EventHandler(this.btnBrowseHearingOptionCAudio_Click);
            RadioButton rbOptionC = new RadioButton { Text = "Correct", Location = new Point(450, 280), Name = "rbOptionCHearing" };
            rbOptionC.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            

            // Option D
            Label lblOptionD = new Label { Text = "Option D", Location = new Point(10, 370) };
            TextBox txtOptionDText = new TextBox { Location = new Point(150, 370), Width = 200 };
            TextBox txtOptionDImage = new TextBox { Location = new Point(150, 400), Width = 200 };
            Button btnBrowseOptionDImage = new Button { Text = "Browse", Location = new Point(360, 400) };
            btnBrowseOptionDImage.Click += new EventHandler(this.btnBrowseHearingOptionDImage_Click);
            TextBox txtOptionDAudio = new TextBox { Location = new Point(150, 430), Width = 200 };
            Button btnBrowseOptionDAudio = new Button { Text = "Browse", Location = new Point(360, 430) };
            btnBrowseOptionDAudio.Click += new EventHandler(this.btnBrowseHearingOptionDAudio_Click);
            RadioButton rbOptionD = new RadioButton { Text = "Correct", Location = new Point(450, 370), Name = "rbOptionDHearing" };
            rbOptionD.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            

            // Time Limit
            Label lblTimeLimit = new Label { Text = "Time Limit (seconds)", Location = new Point(10, 460) };
            TextBox txtTimeLimit = new TextBox { Location = new Point(150, 460), Width = 100, Name = "txtTimeLimit" };

            // Delete Button
            Button btnDeleteHearingQuestion = new Button { Text = "Delete", Location = new Point(150, 610), 
            Size = new Size(80, 30) }; btnDeleteHearingQuestion.Click += new EventHandler(this.btnDeleteHearingQuestion_Click);

            // Reset Button
            Button btnResetHearingQuestions = new Button { Text = "Reset", Location = new Point(240, 610),
                Size = new Size(80, 30) }; 
            btnResetHearingQuestions.Click += new EventHandler(this.btnResetHearingQuestions_Click);
            
            
            // Save Button
            Button btnSaveQuestion = new Button { Text = "Save Hearing Question", Location = new Point(150, 490) };
            btnSaveQuestion.Click += new EventHandler(this.btnSaveHearingQuestion_Click);

            // Back Button
            Button btnBackAddQuestions = new Button { Text = "Back", Location = new Point(300, 470), Size = new Size(80, 30) };
            btnBackAddQuestions.Click += new EventHandler(this.BackButton_Click);

            // ListBox for displaying questions
            ListBox listBoxHearingQuestions = new ListBox { Location = new Point(10, 500),
                Size = new Size(520, 100), Name = "listBoxHearingQuestions" }; 
           

            // Add Controls to Panel
            this.panelAddHearingQuestion.Controls.Add(lblQuestionText);//0
            this.panelAddHearingQuestion.Controls.Add(txtQuestionText);//1
            this.panelAddHearingQuestion.Controls.Add(lblQuestionImage);//2
            this.panelAddHearingQuestion.Controls.Add(txtQuestionImage);//3
            this.panelAddHearingQuestion.Controls.Add(btnBrowseImage);//4
            this.panelAddHearingQuestion.Controls.Add(lblQuestionAudio);//5
            this.panelAddHearingQuestion.Controls.Add(txtQuestionAudio);//6
            this.panelAddHearingQuestion.Controls.Add(btnBrowseAudio);//7
            this.panelAddHearingQuestion.Controls.Add(lblOptionA);//8
            this.panelAddHearingQuestion.Controls.Add(txtOptionAText);//9
            this.panelAddHearingQuestion.Controls.Add(txtOptionAImage);//10
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionAImage);//11
            this.panelAddHearingQuestion.Controls.Add(txtOptionAAudio);//12
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionAAudio);//13
            this.panelAddHearingQuestion.Controls.Add(lblOptionB);//14
            this.panelAddHearingQuestion.Controls.Add(txtOptionBText);//15
            this.panelAddHearingQuestion.Controls.Add(txtOptionBImage);//16
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionBImage);//17
            this.panelAddHearingQuestion.Controls.Add(txtOptionBAudio);//18
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionBAudio);//19
            this.panelAddHearingQuestion.Controls.Add(lblOptionC);//20
            this.panelAddHearingQuestion.Controls.Add(txtOptionCText);//21
            this.panelAddHearingQuestion.Controls.Add(txtOptionCImage);//22
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionCImage);//23
            this.panelAddHearingQuestion.Controls.Add(txtOptionCAudio);//24
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionCAudio);//25
            this.panelAddHearingQuestion.Controls.Add(lblOptionD);//26
            this.panelAddHearingQuestion.Controls.Add(txtOptionDText);//27
            this.panelAddHearingQuestion.Controls.Add(txtOptionDImage);//28
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionDImage);//29
            this.panelAddHearingQuestion.Controls.Add(txtOptionDAudio);//30
            this.panelAddHearingQuestion.Controls.Add(btnBrowseOptionDAudio);//31
            this.panelAddHearingQuestion.Controls.Add(lblTimeLimit); //32
            this.panelAddHearingQuestion.Controls.Add(txtTimeLimit);//33
            this.panelAddHearingQuestion.Controls.Add(btnSaveQuestion);//34
            this.panelAddHearingQuestion.Controls.Add(btnBackAddQuestions);//35
            this.panelAddHearingQuestion.Controls.Add(rbOptionA);//36
            this.panelAddHearingQuestion.Controls.Add(rbOptionB);//37
            this.panelAddHearingQuestion.Controls.Add(rbOptionC);//38
            this.panelAddHearingQuestion.Controls.Add(rbOptionD);//39
            this.panelAddHearingQuestion.Controls.Add(listBoxHearingQuestions);//40
            this.panelAddHearingQuestion.Controls.Add(btnDeleteHearingQuestion);
            this.panelAddHearingQuestion.Controls.Add(btnResetHearingQuestions);


            LoadHearingQuestionsIntoListBox();
        }

        private void btnSaveHearingQuestion_Click(object sender, EventArgs e)
        {
            string tableName = $"HearingQuestions{selectedDay}";

            string questionText = ((TextBox)this.panelAddHearingQuestion.Controls[1]).Text;
            byte[] questionImage = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[3])?.Text);
            byte[] questionAudio = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[6])?.Text);

            // Option A
            string optionAText = ((TextBox)this.panelAddHearingQuestion.Controls[9]).Text;
            byte[] optionAImage = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[10])?.Text);
            byte[] optionAAudio = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[12])?.Text);

            // Option B
            string optionBText = ((TextBox)this.panelAddHearingQuestion.Controls[15]).Text;
            byte[] optionBImage = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[16])?.Text);
            byte[] optionBAudio = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[18])?.Text);

            // Option C
            string optionCText = ((TextBox)this.panelAddHearingQuestion.Controls[21]).Text;
            byte[] optionCImage = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[22])?.Text);
            byte[] optionCAudio = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[24])?.Text);

            // Option D
            string optionDText = ((TextBox)this.panelAddHearingQuestion.Controls[27]).Text;
            byte[] optionDImage = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[28])?.Text);
            byte[] optionDAudio = GetFileDataSafe(((TextBox)this.panelAddHearingQuestion.Controls[30])?.Text);

            // Determine Correct Answer
            string correctAnswer = string.Empty;
            if (((RadioButton)this.panelAddHearingQuestion.Controls["rbOptionAHearing"]).Checked) correctAnswer = "A";
            if (((RadioButton)this.panelAddHearingQuestion.Controls["rbOptionBHearing"]).Checked) correctAnswer = "B";
            if (((RadioButton)this.panelAddHearingQuestion.Controls["rbOptionCHearing"]).Checked) correctAnswer = "C";
            if (((RadioButton)this.panelAddHearingQuestion.Controls["rbOptionDHearing"]).Checked) correctAnswer = "D";

            // Include time limit along with other fields
            int timeLimit = int.Parse(((TextBox)this.panelAddHearingQuestion.Controls[33]).Text);

            // Add @TimeLimit parameter to the SQL query
            string query = $@"
    INSERT INTO {tableName} (QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer, TimeLimit) 
    VALUES (@QuestionText, CONVERT(VARBINARY(MAX), @QuestionImage), CONVERT(VARBINARY(MAX), @QuestionAudio), 
    @OptionAText, CONVERT(VARBINARY(MAX), @OptionAImage), CONVERT(VARBINARY(MAX), @OptionAAudio), 
    @OptionBText, CONVERT(VARBINARY(MAX), @OptionBImage), CONVERT(VARBINARY(MAX), @OptionBAudio), 
    @OptionCText, CONVERT(VARBINARY(MAX), @OptionCImage), CONVERT(VARBINARY(MAX), @OptionCAudio), 
    @OptionDText, CONVERT(VARBINARY(MAX), @OptionDImage), CONVERT(VARBINARY(MAX), @OptionDAudio), 
    @CorrectAnswer, @TimeLimit)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add all parameters
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
                command.Parameters.AddWithValue("@TimeLimit", (object)timeLimit ?? DBNull.Value);

                OpenConnection();
                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Hearing question saved successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving hearing question: {ex.Message}");
                }
                finally
                {
                    CloseConnection();
                }
            }
            LoadHearingQuestionsIntoListBox();
            LoadHearingQuestionCount();
        }

        private void InitializePanelAddReadingQuestions()
        {
            panelAddReadingQuestion = new Panel { 
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelAddReadingQuestion);

            // Question Text
            Label lblQuestionText = new Label { Text = "Question Text", Location = new Point(10, 10) };
            TextBox txtQuestionText = new TextBox { Location = new Point(150, 10), Width = 200 };

            // Question Image
            Label lblQuestionImage = new Label { Text = "Question Image", Location = new Point(10, 40) };
            TextBox txtQuestionImage = new TextBox { Location = new Point(150, 40), Width = 200 };
            Button btnBrowseImage = new Button { Text = "Browse", Location = new Point(360, 40) };
            btnBrowseImage.Click += new EventHandler(this.btnBrowseReadingQuestionImage_Click);

            // Question Audio
            Label lblQuestionAudio = new Label { Text = "Question Audio", Location = new Point(10, 70) };
            TextBox txtQuestionAudio = new TextBox { Location = new Point(150, 70), Width = 200 };
            Button btnBrowseAudio = new Button { Text = "Browse", Location = new Point(360, 70) };
            btnBrowseAudio.Click += new EventHandler(this.btnBrowseReadingQuestionAudio_Click);

            // Option A
            Label lblOptionA = new Label { Text = "Option A", Location = new Point(10, 100) };
            TextBox txtOptionAText = new TextBox { Location = new Point(150, 100), Width = 200 };
            TextBox txtOptionAImage = new TextBox { Location = new Point(150, 130), Width = 200 };
            Button btnBrowseOptionAImage = new Button { Text = "Browse", Location = new Point(360, 130) };
            btnBrowseOptionAImage.Click += new EventHandler(this.btnBrowseReadingOptionAImage_Click);
            TextBox txtOptionAAudio = new TextBox { Location = new Point(150, 160), Width = 200 };
            Button btnBrowseOptionAAudio = new Button { Text = "Browse", Location = new Point(360, 160) };
            btnBrowseOptionAAudio.Click += new EventHandler(this.btnBrowseReadingOptionAAudio_Click);
            RadioButton rbOptionA = new RadioButton { Text = "Correct", Location = new Point(450, 100), Name = "rbOptionAReading" };
            rbOptionA.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddReadingQuestion.Controls.Add(rbOptionA);//0

            // Option B
            Label lblOptionB = new Label { Text = "Option B", Location = new Point(10, 190) };
            TextBox txtOptionBText = new TextBox { Location = new Point(150, 190), Width = 200 };
            TextBox txtOptionBImage = new TextBox { Location = new Point(150, 220), Width = 200 };
            Button btnBrowseOptionBImage = new Button { Text = "Browse", Location = new Point(360, 220) };
            btnBrowseOptionBImage.Click += new EventHandler(this.btnBrowseReadingOptionBImage_Click);
            TextBox txtOptionBAudio = new TextBox { Location = new Point(150, 250), Width = 200 };
            Button btnBrowseOptionBAudio = new Button { Text = "Browse", Location = new Point(360, 250) };
            btnBrowseOptionBAudio.Click += new EventHandler(this.btnBrowseReadingOptionBAudio_Click);
            RadioButton rbOptionB = new RadioButton { Text = "Correct", Location = new Point(450, 190), Name = "rbOptionBReading" };
            rbOptionB.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddReadingQuestion.Controls.Add(rbOptionB);//1

            // Option C
            Label lblOptionC = new Label { Text = "Option C", Location = new Point(10, 280) };
            TextBox txtOptionCText = new TextBox { Location = new Point(150, 280), Width = 200 };
            TextBox txtOptionCImage = new TextBox { Location = new Point(150, 310), Width = 200 };
            Button btnBrowseOptionCImage = new Button { Text = "Browse", Location = new Point(360, 310) };
            btnBrowseOptionCImage.Click += new EventHandler(this.btnBrowseReadingOptionCImage_Click);
            TextBox txtOptionCAudio = new TextBox { Location = new Point(150, 340), Width = 200 };
            Button btnBrowseOptionCAudio = new Button { Text = "Browse", Location = new Point(360, 340) };
            btnBrowseOptionCAudio.Click += new EventHandler(this.btnBrowseReadingOptionCAudio_Click);
            RadioButton rbOptionC = new RadioButton { Text = "Correct", Location = new Point(450, 280), Name = "rbOptionCReading" };
            rbOptionC.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddReadingQuestion.Controls.Add(rbOptionC);//2

            // Option D
            Label lblOptionD = new Label { Text = "Option D", Location = new Point(10, 370) };
            TextBox txtOptionDText = new TextBox { Location = new Point(150, 370), Width = 200 };
            TextBox txtOptionDImage = new TextBox { Location = new Point(150, 400), Width = 200 };
            Button btnBrowseOptionDImage = new Button { Text = "Browse", Location = new Point(360, 400) };
            btnBrowseOptionDImage.Click += new EventHandler(this.btnBrowseReadingOptionDImage_Click);
            TextBox txtOptionDAudio = new TextBox { Location = new Point(150, 430), Width = 200 };
            Button btnBrowseOptionDAudio = new Button { Text = "Browse", Location = new Point(360, 430) };
            btnBrowseOptionDAudio.Click += new EventHandler(this.btnBrowseReadingOptionDAudio_Click);
            RadioButton rbOptionD = new RadioButton { Text = "Correct", Location = new Point(450, 370), Name = "rbOptionDReading" };
            rbOptionD.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.panelAddReadingQuestion.Controls.Add(rbOptionD);//3

            // Time Limit
            Label lblTimeLimit = new Label { Text = "Time Limit (seconds)", Location = new Point(10, 460) };
            TextBox txtTimeLimit = new TextBox { Location = new Point(150, 460), Width = 100, Name = "txtTimeLimit" };

            // Delete Button
            Button btnDeleteReadingQuestion = new Button { Text = "Delete", Location = new Point(150, 610), 
                Size = new Size(80, 30) }; btnDeleteReadingQuestion.Click += new EventHandler(this.btnDeleteReadingQuestion_Click);
           
            // Reset Button
            Button btnResetReadingQuestions = new Button { Text = "Reset", Location = new Point(240, 610), 
                Size = new Size(80, 30) };
            btnResetReadingQuestions.Click += new EventHandler(this.btnResetReadingQuestions_Click);

            // Save Button
            Button btnSaveQuestion = new Button { Text = "Save Reading Question", Location = new Point(150, 490) };
            btnSaveQuestion.Click += new EventHandler(this.btnSaveReadingQuestion_Click);

            // Back Button
            Button btnBackAddQuestions = new Button { Text = "Back", Location = new Point(300, 470), Size = new Size(80, 30) };
            btnBackAddQuestions.Click += new EventHandler(this.BackButton_Click);

            // ListBox for displaying questions
            ListBox listBoxReadingQuestions = new ListBox { Location = new Point(10, 500), 
                Size = new Size(520, 100), Name = "listBoxReadingQuestions" }; 
           

            // Add Controls to Panel
            this.panelAddReadingQuestion.Controls.Add(lblQuestionText);//4
            this.panelAddReadingQuestion.Controls.Add(txtQuestionText);//5
            this.panelAddReadingQuestion.Controls.Add(lblQuestionImage);//6
            this.panelAddReadingQuestion.Controls.Add(txtQuestionImage);//7
            this.panelAddReadingQuestion.Controls.Add(btnBrowseImage);//8
            this.panelAddReadingQuestion.Controls.Add(lblQuestionAudio);//9
            this.panelAddReadingQuestion.Controls.Add(txtQuestionAudio);//10

            this.panelAddReadingQuestion.Controls.Add(btnBrowseAudio);//11
            this.panelAddReadingQuestion.Controls.Add(lblOptionA);//12
            this.panelAddReadingQuestion.Controls.Add(txtOptionAText);//13
            this.panelAddReadingQuestion.Controls.Add(txtOptionAImage);//14
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionAImage);//15
            this.panelAddReadingQuestion.Controls.Add(txtOptionAAudio);//16

            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionAAudio);//17
            this.panelAddReadingQuestion.Controls.Add(lblOptionB);//18
            this.panelAddReadingQuestion.Controls.Add(txtOptionBText);//19
            this.panelAddReadingQuestion.Controls.Add(txtOptionBImage);//20
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionBImage);//21
            this.panelAddReadingQuestion.Controls.Add(txtOptionBAudio);//22
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionBAudio);//23
            this.panelAddReadingQuestion.Controls.Add(lblOptionC);//24
            this.panelAddReadingQuestion.Controls.Add(txtOptionCText);//25
            this.panelAddReadingQuestion.Controls.Add(txtOptionCImage);//26
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionCImage);//27
            this.panelAddReadingQuestion.Controls.Add(txtOptionCAudio);//28
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionCAudio);//29
            this.panelAddReadingQuestion.Controls.Add(lblOptionD);//30
            this.panelAddReadingQuestion.Controls.Add(txtOptionDText);//31
            this.panelAddReadingQuestion.Controls.Add(txtOptionDImage);//32
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionDImage);//33
            this.panelAddReadingQuestion.Controls.Add(txtOptionDAudio);//34
            this.panelAddReadingQuestion.Controls.Add(btnBrowseOptionDAudio);//35
            this.panelAddReadingQuestion.Controls.Add(lblTimeLimit); //36
            this.panelAddReadingQuestion.Controls.Add(txtTimeLimit);//37
            this.panelAddReadingQuestion.Controls.Add(btnSaveQuestion);
            this.panelAddReadingQuestion.Controls.Add(btnBackAddQuestions);
            this.panelAddReadingQuestion.Controls.Add(listBoxReadingQuestions);
            this.panelAddReadingQuestion.Controls.Add(btnDeleteReadingQuestion);
            this.panelAddReadingQuestion.Controls.Add(btnResetReadingQuestions);
            LoadReadingQuestionsIntoListBox();
        }
 
     
        private void btnSaveReadingQuestion_Click(object sender, EventArgs e)
        {
            string tableName = $"ReadingQuestions{selectedDay}";

            string questionText = ((TextBox)this.panelAddReadingQuestion.Controls[5]).Text;
            byte[] questionImage = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[7])?.Text);
            byte[] questionAudio = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[10])?.Text);

            // Option A
            string optionAText = ((TextBox)this.panelAddReadingQuestion.Controls[13]).Text;
            byte[] optionAImage = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[14])?.Text);
            byte[] optionAAudio = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[16])?.Text);

            // Option B
            string optionBText = ((TextBox)this.panelAddReadingQuestion.Controls[19]).Text;
            byte[] optionBImage = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[20])?.Text);
            byte[] optionBAudio = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[22])?.Text);

            // Option C
            string optionCText = ((TextBox)this.panelAddReadingQuestion.Controls[25]).Text;
            byte[] optionCImage = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[26])?.Text);
            byte[] optionCAudio = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[28])?.Text);

            // Option D
            string optionDText = ((TextBox)this.panelAddReadingQuestion.Controls[31]).Text;
            byte[] optionDImage = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[32])?.Text);
            byte[] optionDAudio = GetFileDataSafe(((TextBox)this.panelAddReadingQuestion.Controls[34])?.Text);

            // Determine Correct Answer
            string correctAnswer = string.Empty;
            if (((RadioButton)this.panelAddReadingQuestion.Controls["rbOptionAReading"]).Checked) correctAnswer = "A";
            if (((RadioButton)this.panelAddReadingQuestion.Controls["rbOptionBReading"]).Checked) correctAnswer = "B";
            if (((RadioButton)this.panelAddReadingQuestion.Controls["rbOptionCReading"]).Checked) correctAnswer = "C";
            if (((RadioButton)this.panelAddReadingQuestion.Controls["rbOptionDReading"]).Checked) correctAnswer = "D";

            // Include time limit along with other fields
            int timeLimit = int.Parse(((TextBox)this.panelAddReadingQuestion.Controls[37]).Text);

            // Add @TimeLimit parameter to the SQL query
            string query = $@"
    INSERT INTO {tableName} (QuestionText, QuestionImage, QuestionAudio, OptionA, OptionAImage, OptionAAudio, OptionB, OptionBImage, OptionBAudio, OptionC, OptionCImage, OptionCAudio, OptionD, OptionDImage, OptionDAudio, CorrectAnswer, TimeLimit) 
    VALUES (@QuestionText, CONVERT(VARBINARY(MAX), @QuestionImage), CONVERT(VARBINARY(MAX), @QuestionAudio), 
    @OptionAText, CONVERT(VARBINARY(MAX), @OptionAImage), CONVERT(VARBINARY(MAX), @OptionAAudio), 
    @OptionBText, CONVERT(VARBINARY(MAX), @OptionBImage), CONVERT(VARBINARY(MAX), @OptionBAudio), 
    @OptionCText, CONVERT(VARBINARY(MAX), @OptionCImage), CONVERT(VARBINARY(MAX), @OptionCAudio), 
    @OptionDText, CONVERT(VARBINARY(MAX), @OptionDImage), CONVERT(VARBINARY(MAX), @OptionDAudio), 
    @CorrectAnswer, @TimeLimit)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add all parameters
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
                command.Parameters.AddWithValue("@TimeLimit", (object)timeLimit ?? DBNull.Value);

                OpenConnection();
                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Reading question saved successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving reading question: {ex.Message}");
                }
                finally
                {
                    CloseConnection();
                }
            }
            LoadReadingQuestionsIntoListBox();
            LoadReadingQuestionCount();
        }
     

        private void LoadHearingQuestionsIntoListBox()
        {
            string tableName = $"HearingQuestions{selectedDay}";

            ListBox listBoxHearingQuestions = (ListBox)this.panelAddHearingQuestion.Controls["listBoxHearingQuestions"];
            if (listBoxHearingQuestions != null)
            {
                listBoxHearingQuestions.Items.Clear();
                string query = $"SELECT ID, QuestionText FROM {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listBoxHearingQuestions.Items.Add($"ID: {reader["ID"]}, Question: {reader["QuestionText"]}");
                    }
                    reader.Close();
                    CloseConnection();
                }
            }
        }

        private void LoadReadingQuestionsIntoListBox()
        {
            string tableName = $"ReadingQuestions{selectedDay}";

            ListBox listBoxReadingQuestions = (ListBox)this.panelAddReadingQuestion.Controls["listBoxReadingQuestions"];
            if (listBoxReadingQuestions != null)
            {
                listBoxReadingQuestions.Items.Clear();
                string query = $"SELECT ID, QuestionText FROM {tableName}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    OpenConnection();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listBoxReadingQuestions.Items.Add($"ID: {reader["ID"]}, Question: {reader["QuestionText"]}");
                    }
                    reader.Close();
                    CloseConnection();
                }
            }
        }
        private void btnDeleteHearingQuestion_Click(object sender, EventArgs e)
        {
            ListBox listBoxHearingQuestions = (ListBox)this.panelAddHearingQuestion.Controls["listBoxHearingQuestions"];
            if (listBoxHearingQuestions != null && listBoxHearingQuestions.SelectedItem != null)
            {
                string selectedText = listBoxHearingQuestions.SelectedItem.ToString();
                int questionId = int.Parse(selectedText.Split(new[] { "ID: " }, StringSplitOptions.None)[1].Split(',')[0]);

                string tableName = $"HearingQuestions{selectedDay}";
                string query = $"DELETE FROM {tableName} WHERE ID = @QuestionID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QuestionID", questionId);
                    OpenConnection();
                    command.ExecuteNonQuery();
                    CloseConnection();
                }

                LoadHearingQuestionsIntoListBox();
            }
        }
        private void btnDeleteReadingQuestion_Click(object sender, EventArgs e)
        {
            ListBox listBoxReadingQuestions = (ListBox)this.panelAddReadingQuestion.Controls["listBoxReadingQuestions"];
            if (listBoxReadingQuestions != null && listBoxReadingQuestions.SelectedItem != null)
            {
                string selectedText = listBoxReadingQuestions.SelectedItem.ToString();
                int questionId = int.Parse(selectedText.Split(new[] { "ID: " }, StringSplitOptions.None)[1].Split(',')[0]);

                string tableName = $"ReadingQuestions{selectedDay}";
                string query = $"DELETE FROM {tableName} WHERE ID = @QuestionID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@QuestionID", questionId);
                    OpenConnection();
                    command.ExecuteNonQuery();
                    CloseConnection();
                }

                LoadReadingQuestionsIntoListBox();
            }
        }

        private void LoadHearingQuestionCount()
        {
            string tableName = $"HearingQuestions{selectedDay}";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int questionCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblQuestionCount.Text = $"Number of Hearing Questions: {questionCount}";
                countHQ = questionCount;
            }
        }
        private void LoadReadingQuestionCount()
        {
            string tableName = $"ReadingQuestions{selectedDay}";
            string query = $"SELECT COUNT(*) FROM {tableName}";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                int questionCount = (int)command.ExecuteScalar();
                CloseConnection();
                lblQuestionCount.Text = $"Number of Reading Questions: {questionCount}";
                countRQ = questionCount;
            }
        }
       
        private void DisplayHearingQuestion(SqlDataReader reader)
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
        private void DisplayReadingQuestion(SqlDataReader reader)
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
        private bool CheckHearingAnswer(string selectedAnswer, int questionNumber)
        {
            string tableName = $"HearingQuestions{selectedDay}";

            string query = $"SELECT CorrectAnswer FROM {tableName} WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                string correctAnswer = command.ExecuteScalar().ToString();
                CloseConnection();

                return selectedAnswer == correctAnswer;
            }
        }
        private bool CheckReadingAnswer(string selectedAnswer, int questionNumber)
        {
            string tableName = $"ReadingQuestions{selectedDay}";

            string query = $"SELECT CorrectAnswer FROM {tableName} WHERE ID = @Number";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Number", questionNumber);
                OpenConnection();
                string correctAnswer = command.ExecuteScalar().ToString();
                CloseConnection();

                return selectedAnswer == correctAnswer;
            }
        }

        private void StartHearingTimer(int timeLimit)
        {
            hearingTimeRemaining = timeLimit;
            hearingQuestionTimer.Start();
        }

        private void StartReadingTimer(int timeLimit)
        {
            readingTimeRemaining = timeLimit;
            readingQuestionTimer.Start();
        }
        private void HearingTimer_Tick(object sender, EventArgs e)
        {
            hearingTimeRemaining--;
            if (hearingTimeRemaining == 10) { warningSoundPlayer.Play(); }

            if (hearingTimeRemaining <= 0)
            {
                hearingQuestionTimer.Stop();
                MessageBox.Show("Time's up for the hearing question!");
               
            }
            else
            {
                lblTime.Text = $"Time remaining for hearing question: {hearingTimeRemaining} seconds";
            }
        }


        private void ReadingTimer_Tick(object sender, EventArgs e)
        {
            readingTimeRemaining--;
            if (readingTimeRemaining == 10) { warningSoundPlayer.Play(); }
            if (readingTimeRemaining <= 0)
            {
                readingQuestionTimer.Stop();
                MessageBox.Show("Time's up for the reading question!");
                // Implement logic for what happens when time is up
            }
            else
            {
                lblTime.Text = $"Time remaining for reading question: {readingTimeRemaining} seconds";
            }
        }

        private void LoadHearingQuestion(int questionNumber)
        {
            string tableName = $"HearingQuestions{selectedDay}";

            string query = $"SELECT * FROM {tableName} WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);

                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    DisplayHearingQuestion(reader);
                    int timeLimit = (int)reader["TimeLimit"];
                    StartHearingTimer(timeLimit);
                }
                reader.Close();
                CloseConnection();
            }

            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }


        private void LoadReadingQuestion(int questionNumber)
        {
            string tableName = $"ReadingQuestions{selectedDay}";

            string query = $"SELECT * FROM {tableName} WHERE ID = @QuestionID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@QuestionID", questionNumber);

                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    DisplayReadingQuestion(reader);
                    int timeLimit = (int)reader["TimeLimit"];
                    StartReadingTimer(timeLimit);
                }
                reader.Close();
                CloseConnection();
            }

            this.panelQuestionSelection.Visible = false;
            this.panelQuestions.Visible = true;
        }


        private void btnAddHearingQuestions_Click(object sender, EventArgs e)
        {
            InitializePanelAddHearingQuestions(); 
            HideAllPanels();
            panelAddHearingQuestion.Visible = true;
        }

        private void btnAddReadingQuestions_Click(object sender, EventArgs e)
        {
            InitializePanelAddReadingQuestions();
            HideAllPanels();
            panelAddReadingQuestion.Visible = true;
        }

        private void LoadSeatingArrangement()
        {
            seatingOrder = new List<int>();
            string query = $"SELECT SeatNumber FROM SeatingArrangements{selectedDay} ORDER BY SeatNumber";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    seatingOrder.Add((int)reader["SeatNumber"]);
                }
                reader.Close();
                CloseConnection();
            }
        }
        private void LoadQuestions()
        {
            LoadHearingQuestionsIntoListBox();
            LoadReadingQuestionsIntoListBox();
            LoadQuestionsIntoListBox();
        }


        private void btnStartQuiz_Click(object sender, EventArgs e)
        {
            isQuizMode = true;
            LoadSeatingArrangement();

            if (seatingOrder == null || seatingOrder.Count == 0)
            {
                MessageBox.Show("Please use the wheel to assign schools to seats first.");
                LoadSchoolsIntoWheel();
                panelMenu.Visible = true;
                return;
            }

            InitializeScores(); // Initialize scores for each school

            InitializePanelQuestionSelection();
            ResetQuestions();
            LoadQuestions();
            currentSeat = 0; // Start with the first seat
            DisplayCurrentTurn();

            panelMenu.Visible = false;
            panelQuestionSelection.Visible = true;
        }


        private void HandleAnswering(int questionIndex, Button questionButton)
        {
            int seatNumber = seatingOrder[currentSeat];
            string currentSchool = GetSchoolBySeatNumber(seatNumber);

            // Simulate checking answer
            string selectedAnswer = ""; // Retrieve selected answer from the interface
            bool isCorrect = CheckAnswer(selectedAnswer, questionIndex);

            if (isCorrect)
            {
                scores[currentSchool]++;
                UpdateScoreInDatabase(currentSchool, scores[currentSchool]);
                MessageBox.Show($"{currentSchool} answered correctly! Score: {scores[currentSchool]}");
            }
            else
            {
                MessageBox.Show($"{currentSchool} answered incorrectly.");
            }

            MarkQuestionAsAnswered(questionIndex);

            // Move to the next turn
            currentSeat = (currentSeat + 1) % seatingOrder.Count;
            DisplayCurrentTurn();
        }


        private string GetSchoolBySeatNumber(int seatNumber)
        {
            string query = $"SELECT SchoolId FROM SeatingArrangement{selectedDay} WHERE SeatNumber = @SeatNumber";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SeatNumber", seatNumber);
                OpenConnection();
                string schoolId = (string)command.ExecuteScalar();
                CloseConnection();

                return schoolId;
            }
        }
        private void DisplayCurrentTurn()
        {
            int seatNumber = seatingOrder[currentSeat];
            string currentSchool = GetSchoolBySeatNumber(seatNumber);

            Label lblCurrentTurn = new Label
            {
                Text = $"Current Turn: {currentSchool} (Seat {seatNumber})",
                Location = new Point(10, 350), // Adjust location as needed
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            panelQuestionSelection.Controls.Add(lblCurrentTurn);
        }
        private void ResetQuestions()
        {
            ResetQuestionTable("HearingQuestions");
            ResetQuestionTable("ReadingQuestions");
            ResetQuestionTable("NormalQuestions");
        }

        private void ResetQuestionTable(string tableName)
        {
            string query = $"UPDATE {tableName}{selectedDay} SET IsAnswered = 0";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        

        private void btnViewQuestions_Click(object sender, EventArgs e)
        {
            isQuizMode = false;
            InitializePanelQuestionSelection();
            LoadQuestions();
            panelMenu.Visible = false;
            panelQuestionSelection.Visible = true;
        }

        private void btnEndQuiz_Click(object sender, EventArgs e)
        {
            // Optionally stop any ongoing timers
            StopAllTimers();


            // Navigate back to the main menu or another appropriate panel
            HideAllPanels();
            panelMenu.Visible = true;
        }

        private void StopAllTimers()
        {
            if (hearingQuestionTimer != null && hearingQuestionTimer.Enabled)
            {
                hearingQuestionTimer.Stop();
            }

            if (readingQuestionTimer != null && readingQuestionTimer.Enabled)
            {
                readingQuestionTimer.Stop();
            }

            if (questionTimer != null && questionTimer.Enabled)
            {
                questionTimer.Stop();
            }
        }
        private void UpdateScoreInDatabase(string schoolId, int newScore)
        {
            string query = $"UPDATE Scores SET Score = @Score WHERE SchoolId = @SchoolId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Score", newScore);
                command.Parameters.AddWithValue("@SchoolId", schoolId);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }



    }
}

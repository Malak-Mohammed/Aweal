using Aweal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class WheelControl : Control
{
    private List<string> items;
    private Random random;
    private float angle;
    private Timer spinTimer;
    private float speed;
    private int seatNumber;
    private SqlConnection connection;
    private List<Color> colors;
   
        private int selectedDay;

        public int SelectedDay
        {
            get { return selectedDay; }
            set { selectedDay = value; }
        }

    public WheelControl(SqlConnection dbConnection)
    {
        items = new List<string>();
        random = new Random();
        this.DoubleBuffered = true;
        this.angle = 0;
        this.speed = 0;
        this.seatNumber = 1;
        this.connection = dbConnection;

        // Define a list of colors to be used for the segments
        colors = new List<Color>
        {
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue,
            Color.Indigo, Color.Violet, Color.Cyan, Color.Magenta, Color.Pink,
            Color.Brown, Color.Purple, Color.Teal, Color.Lime, Color.Salmon
        };

        spinTimer = new Timer();
        spinTimer.Interval = 10;
        spinTimer.Tick += SpinTimer_Tick;
    }

    public void AddItem(string item)
    {
        items.Add(item);
        this.Invalidate();
    }

    public void Spin()
    {
        if (items.Count == 0) return;

        speed = 15; // Adjust speed as needed
        spinTimer.Start();
    }

    private void SpinTimer_Tick(object sender, EventArgs e)
    {
        angle += speed;
        speed *= 0.98f; // Slow down the spin gradually
        if (speed < 0.1f)
        {
            speed = 0;
            spinTimer.Stop();
            OnSpinEnd();
        }
        this.Invalidate();
    }

    private void OnSpinEnd()
    {
        // Calculate the selected index based on the final angle
        float segmentAngle = 360.0f / items.Count;
        float adjustedAngle = angle % 360; // Normalize the angle to [0, 360)
        int selectedIndex = (int)((adjustedAngle + segmentAngle / 2) % 360 / segmentAngle);
        string selectedItem = items[selectedIndex];

        // Assign seat, remove selected item, and insert into database
        AssignSeat(selectedItem);
        items.RemoveAt(selectedIndex);
        this.Invalidate();

        // Check if all seats are assigned
        if (items.Count == 0)
        {
            MessageBox.Show("All seats have been assigned!");
        }
    }

    private void AssignSeat(string school)
    {
        // Retrieve SchoolID from the appropriate Schools table
        int schoolID = GetSchoolID(school);
        string seatingTableName = $"SeatingArrangements{selectedDay}";

        string insertQuery = $"INSERT INTO {seatingTableName} (SeatNumber, SchoolID) VALUES (@SeatNumber, @SchoolID)";
        using (SqlCommand command = new SqlCommand(insertQuery, connection))
        {
            command.Parameters.AddWithValue("@SeatNumber", seatNumber);
            command.Parameters.AddWithValue("@SchoolID", schoolID);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        MessageBox.Show($"Assigned {school} to Seat {seatNumber}");
        seatNumber++;
    }


    private int GetSchoolID(string schoolName)
    {
        int schoolID = 0;
        string tableName = $"Schools{selectedDay}";
        string query = $"SELECT ID FROM {tableName} WHERE Name = @Name";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Name", schoolName);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                schoolID = (int)reader["ID"];
            }
            reader.Close();
            connection.Close();
        }
        return schoolID;
    }


    public void Reset()
    {
        items.Clear();
        seatNumber = 1;
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (items.Count == 0) return;

        float sweepAngle = 360.0f / items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            e.Graphics.FillPie(new SolidBrush(colors[i % colors.Count]), ClientRectangle, angle + i * sweepAngle, sweepAngle);
            e.Graphics.DrawPie(Pens.Black, ClientRectangle, angle + i * sweepAngle, sweepAngle);

            PointF textPoint = GetTextPoint(i, sweepAngle);
            e.Graphics.DrawString(items[i], this.Font, Brushes.Black, textPoint);
        }

        // Draw the arrow
        DrawArrow(e.Graphics);
    }

    private PointF GetTextPoint(int index, float sweepAngle)
    {
        double radians = (angle + index * sweepAngle + sweepAngle / 2) * Math.PI / 180.0;
        float x = (float)(this.Width / 2 + Math.Cos(radians) * (this.Width / 3 + 20));
        float y = (float)(this.Height / 2 + Math.Sin(radians) * (this.Height / 3 + 20));
        return new PointF(x, y);
    }

    private void DrawArrow(Graphics graphics)
    {
        PointF arrowPoint1 = new PointF(this.Width / 2 - 10, 5);
        PointF arrowPoint2 = new PointF(this.Width / 2 + 10, 5);
        PointF arrowPoint3 = new PointF(this.Width / 2, 25);

        PointF[] arrowPoints = { arrowPoint1, arrowPoint2, arrowPoint3 };

        graphics.FillPolygon(Brushes.Red, arrowPoints);
        graphics.DrawPolygon(Pens.Black, arrowPoints);
    }
}

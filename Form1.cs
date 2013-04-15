using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NFLParsing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String[] data = File.ReadAllLines(@"C:\nflplays.csv");
            String line;
            
            // Start at 1, skip the header row
            for (long cursor = 1; cursor < data.LongLength; cursor++)
            {
                line = data[cursor];

                CheckForScoringDrive(data, line, cursor);

            }
        }

        private void CheckForScoringDrive(String[] data, String currentPlay, long cursor)
        {
            String[] data_columns;
            String game_id;
            String home_team;
            String away_team;
            String last_play_offense;
            int offensive_score = 0;
            int defensive_score = 0;
            String year;
            String month;
            String day;
            StringBuilder home_sb;
            StringBuilder away_sb;

            data_columns = currentPlay.Split('\t');
            game_id = data_columns[1];

            String[] nextLine = data[cursor+1].Split('\t');
            String new_game_id = nextLine[1];

            if (new_game_id != game_id) // Next line is from a new game.  Record the current game.
            {
                home_team = data_columns[17];
                away_team = data_columns[18];
                last_play_offense = data_columns[5];
                int.TryParse(data_columns[11], out offensive_score);
                int.TryParse(data_columns[12], out defensive_score);
                year = data_columns[14];
                month = data_columns[15];
                day = data_columns[16];

                home_sb = new StringBuilder();
                away_sb = new StringBuilder();

                if (home_team == last_play_offense)
                {
                    home_sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", game_id, home_team, home_team, offensive_score, (offensive_score > defensive_score) ? "W" : "L", year, month, day);
                    away_sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", game_id, home_team, away_team, defensive_score, (defensive_score > offensive_score) ? "W" : "L", year, month, day);
                }
                else
                {
                    home_sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", game_id, home_team, home_team, defensive_score, (offensive_score < defensive_score) ? "W" : "L", year, month, day);
                    away_sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", game_id, home_team, away_team, offensive_score, (defensive_score < offensive_score) ? "W" : "L", year, month, day);
                }

                // Output:
                // Game ID, home team, score, W/L, year, month, day
                // Game ID, away team, score, W/L, year, month, day
                using (StreamWriter sw = File.AppendText(@"C:\WeatherResults.csv"))
                {
                    sw.WriteLine(home_sb.ToString());
                    sw.WriteLine(away_sb.ToString());
                }
            }
        }
    }
}

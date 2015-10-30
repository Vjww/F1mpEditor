using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F1mpEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    var connection = new SavedGameConnection();
        //    connection.Open("", StreamDirectionType.Read);
        //
        //    var playerTeam = connection.ReadInteger(46344);
        //    var playerTeamMotivation = connection.ReadInteger(position + (offset * (playerTeam - 1)));
        //
        //}

        /*
        Offset B508 in saved game file is the number of the team selected!
        Note order of teams is not consistant.

        B488 -> B4D0 (19 x 4 bytes)

        1997
        ----
        Team GameOrder ChampOrder1996 Actual96/97
        Benetton   1 3
        Williams   2 1
        Ferrari    3 2
        McLaren    4 4
        Jordan     5 5
        Sauber     6 6 7/7?
        Prost      7 7 6/6?
        Tyrrell    8 9 8/10?
        Arrows     9 10 9/8?
        Minardi   10 11 10/11?
        Stewart   11 8 N/A/9?
        Lola      12 12
        Forti     13 13
        Larousse  14 14
        Lotus     15 15
        Pacific   16 16
        Honda     17 17
        Simtek    18 18
        TNT       19 19
        */
    }
}

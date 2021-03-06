﻿/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCForge.SQL;
using System.Text.RegularExpressions;


namespace MCForge.Commands
{
	public class CmdSeen : Command
	{
		public override string name { get { return "seen"; } }
		public override string shortcut { get { return ""; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
		public override string type { get { return "mod"; } }
		public CmdSeen() { }

		public override void Use(Player p, string message)
		{
			if ( !Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])") ) {
				if (Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*")) {
					Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
					MatchCollection mc = rg.Matches(message.ToLower());
					if (mc.Count > 0) {
						Match ma = mc[0];
						GroupCollection gc = ma.Groups;
						message.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
					}
				}
			}
			Player pl = Player.Find(message);
			if (pl != null && !pl.hidden)
			{
				Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " is currently online.");
				return;
			}

			using (DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name='" + message + "'"))
			{
				if (playerDb.Rows != null && playerDb.Rows.Count > 0)
					Player.SendMessage(p, message + " was last seen: " + playerDb.Rows[0]["LastLogin"]);
				else
					Player.SendMessage(p, "Unable to find player");
			}
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/seen [player] - says when a player was last seen on the server");
		}
	}
}
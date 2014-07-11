/// Copyright Alexander Lyons 2013, 2014
///
/// This file is part of AI Shell.
///
/// AI Shell is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// AI Shell is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with AI Shell.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Shell
{
    public static class AIShellUtility
    {
        public static List<String> GetDateTimeTriggers(DateTime time)
        {
            List<String> output = new List<String>();

            output.Add(time.ToString("MM/dd"));
            output.Add(time.ToString("ddd"));

            if (output[1].Equals("Sat") || output[1].Equals("Sun"))
                output.Add("Weekend");
            else
                output.Add("Weekday");

            if (output[1].Equals("Mon") || output[1].Equals("Wed") || output[1].Equals("Fri"))
                output.Add("MWF");
            else
                output.Add("TR");

            output.Add("Everyday");

            return output;
        }
    }
}

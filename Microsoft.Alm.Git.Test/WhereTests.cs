﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Microsoft.Alm.Git.Test
{
    public class WhereTests
    {
        private static StringComparer PathComparer = StringComparer.InvariantCultureIgnoreCase;

        public static object[][] FindAppData
        {
            get
            {
                var data = new List<object[]>()
                {
                    new object[] { "cmd" },
                    new object[] { "notepad" },
                    new object[] { "calc" },
                    new object[] { "powershell" },
                    new object[] { "git" },
                };

                return data.ToArray();
            }
        }

        [Theory]
        [MemberData(nameof(FindAppData))]
        public void Where_FindApp(string app)
        {
            string path1;
            Assert.True(CmdWhere(app, out path1));

            string path2;
            Assert.True(Where.FindApp(app, out path2));

            Assert.True(PathComparer.Equals(path1, path2));
        }

        [Fact]
        public void Where_FindGit()
        {
            string gitPath;
            if (!Where.FindApp("git", out gitPath))
                throw new Exception("Git not found on system");

            List<GitInstallation> installations;
            Assert.True(Where.FindGitInstallations(out installations));
            Assert.True(installations.Count > 0);
            Assert.True(PathComparer.Equals(installations[0].Git, gitPath));

            GitInstallation installation;
            Assert.True(Where.FindGitInstallation(installations[0].Path, installations[0].Version, out installation));
            Assert.True(installations[0] == installation);
        }

        private static bool CmdWhere(string app, out string path)
        {
            path = null;

            var startInfo = new ProcessStartInfo
            {
                Arguments = "/c where " + app,
                CreateNoWindow = true,
                FileName = "cmd",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            var process = Process.Start(startInfo);
            if (process.WaitForExit(3000))
            {
                path = process.StandardOutput.ReadLine();
                path = path.Trim();
            }

            return path != null;
        }
    }
}

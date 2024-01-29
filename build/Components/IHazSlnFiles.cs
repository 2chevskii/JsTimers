using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;

interface IHazSlnFiles : INukeBuild
{
    [Solution, Required]
    Solution Sln => TryGetValue(() => Sln);

    Project SrcProject => Sln.GetAllProjects("JsTimers").First();
    Project TestProject => Sln.GetAllProjects("JsTimers.Tests").First();
}

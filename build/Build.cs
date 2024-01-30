using Nuke.Common;

class Build : NukeBuild, ICompile, IClean, IDocs, ITest, IPack
{
    public static int Main() => Execute<Build>(x => x.From<ICompile>().CompileSrc);

    T From<T>()
        where T : class => this as T;
}

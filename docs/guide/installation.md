<script setup>
import pkg from '../package.json';

const { latestReleaseVersion } = pkg;

</script>

# Recommended installation (NuGet package)

The package is [provided on NuGet][nuget-pkg] under the name of `JsTimers`

Latest package version is: <code>{{ latestReleaseVersion }}</code>

## .NET CLI

From within your terminal, use:

<div class="language-sh vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">sh</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">dotnet</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;"> add path/to/your/project.csproj package JsTimers --version </span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">{{ latestReleaseVersion }}</span></span></code></pre></div>

## Visual Studio package manager console

Inside of your package manager console, run the command:

<div class="language-powershell vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">powershell</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">Install-Package</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> JsTimers </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">-</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">Version </span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;">{{ latestReleaseVersion }}</span></span></code></pre></div>

## MSBuild project

Add the following to your project file (ending with `.csproj` or similar):

<div class="language-xml vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">xml</span><pre class="shiki shiki-themes github-light github-dark vp-code"><code><span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">ItemGroup</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">  &lt;</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">PackageReference</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Include</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">=</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">"JsTimers"</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Version</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">=</span><span style="--shiki-light:#032F62;--shiki-dark:#9ECBFF;">"{{ latestReleaseVersion }}"</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> /&gt;</span></span>
<span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;/</span><span style="--shiki-light:#22863A;--shiki-dark:#85E89D;">ItemGroup</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;</span></span></code></pre></div>

# Alternative installation options

You can use those methods to install the dependency if you don't have access
to the NuGet feed, or you want to use version not listed there

Every commit to the [source repository][github-repo] gets built by the CI
and produces artifacts which are ready to use, their installation
is explained below

Builds are performed by the `main` workflow,
which history can be found on [this page][main-workflow-history]

## NuGet package

> Artifact name is: `packages`

1. Download the artifact
2. Extract it into your local folder
which serves as a NuGet source (you have to configure it first)

> Final path should be something along the lines of `/<your-local-source>/JsTimers/<version>/JsTimers.nupkg`

3. Install the package using any of the methods listed above

## DLL reference

> Artifact name is: `libraries`

Alternatively, you can reference a `.dll` directly in your project

1. Download the artifact
2. Extract it into your local folder
which you use for project dependencies like this
3. Reference it in your **MSBuild** project file like this:

```xml
<ItemGroup>
  <Reference Include="$(YourReferenceDir)JsTimers.dll" />
</ItemGroup>
```

[nuget-pkg]: https://nuget.org/packages/JsTimers
[github-repo]: https://github.com/2chevskii/JsTimers
[main-workflow-history]: https://github.com/2chevskii/JsTimers/actions/workflows/main.yml

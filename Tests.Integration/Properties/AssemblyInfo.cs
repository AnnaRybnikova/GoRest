using NUnit.Framework;
using Tests.Integration;

[assembly: LevelOfParallelism(GlobalConstants.LevelOfParallelism)]
[assembly: Parallelizable(ParallelScope.None)]

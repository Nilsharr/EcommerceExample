using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using DatabaseBenchmarks.Benchmarks;
using Perfolizer.Horology;


var config = ManualConfig.Create(DefaultConfig.Instance)
    .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage).WithTimeUnit(TimeUnit.Millisecond));


var userSummary = BenchmarkRunner.Run<UserBenchmark>(config);
var productSummary = BenchmarkRunner.Run<ProductBenchmark>(config);
var orderSummary = BenchmarkRunner.Run<OrderBenchmark>(config);
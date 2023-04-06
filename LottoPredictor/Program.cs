using HtmlAgilityPack;

public class LotteryResult {
    public DateTime DrawDate { get; set; }
    public int[] WinningNumbers { get; set; }
    public decimal JackpotPrize { get; set; }
    }

public class LotteryPredictor {
    private static readonly Random random = new Random();

    public static int[] GenerateRandomCombination(int minValue, int maxValue, int count) {
        var numbers = new int[count];
        for (int i = 0; i < count; i++) {
            int randomNumber;
            do {
                randomNumber = random.Next(minValue, maxValue + 1);
                } while (Array.IndexOf(numbers, randomNumber) != -1);

            numbers[i] = randomNumber;
            }

        Array.Sort(numbers);
        return numbers;
        }
    }

public class Program {

    public static async Task Main() {
        var url2022 = "https://www.lottopcso.com/6-45-lotto-result-history-summary-year-2022/";
        var lotteryResults2022 = await ExtractLotteryResults(url2022);
        var url2023 = "https://www.lottopcso.com/6-45-lotto-result-history-and-summary/";
        var lotteryResults2023 = await ExtractLotteryResults(url2023);

        var lotteryResults = new List<LotteryResult>();
        lotteryResults.AddRange(lotteryResults2022);
        lotteryResults.AddRange(lotteryResults2023);

        foreach (var result in lotteryResults.OrderBy(c => c.DrawDate)) {
            Console.WriteLine($"Draw Date: {result.DrawDate.Date}, Winning Numbers: {string.Join(", ", result.WinningNumbers)}, Jackpot Prize (Php): {result.JackpotPrize}");
            }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Prediction for next 30 days");
        int minValue = 1;
        int maxValue = lotteryResults
            .SelectMany(result => result.WinningNumbers)
            .Max();

        int numberCount = 6;
        int days = 30;

        for (int i = 0; i < days; i++) {
            var randomCombination = LotteryPredictor.GenerateRandomCombination(minValue, maxValue, numberCount);
            Console.WriteLine($"Day {i + 1}: {string.Join(", ", randomCombination)}");
            }
        Console.ReadLine();
        }

    public static async Task<List<LotteryResult>> ExtractLotteryResults(string url) {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        var table = htmlDocument.DocumentNode.SelectSingleNode("//figure[contains(@class, 'wp-block-table')]/table");

        var lotteryResults = new List<LotteryResult>();

        if (table != null) {
            var rows = table.SelectNodes("tbody/tr");
            if (rows != null) {
                foreach (var row in rows) {
                    var columns = row.SelectNodes("td");
                    if (columns.Count >= 3) {
                        var drawDateString = columns[0].InnerText.Trim();
                        var winningNumbersString = columns[1].InnerText.Trim();
                        var jackpotPrizeString = columns[2].InnerText.Trim().Replace("Php", "").Trim();

                        if (!string.IsNullOrWhiteSpace(winningNumbersString)) {

                            try {
                                DateTime.TryParse(drawDateString, out DateTime drawDate);
                                decimal.TryParse(jackpotPrizeString, out decimal jackpotPrize);

                                var winningNumbers = Array.ConvertAll(winningNumbersString.Split('-'), int.Parse);

                                lotteryResults.Add(new LotteryResult {
                                    DrawDate = drawDate,
                                    WinningNumbers = winningNumbers,
                                    JackpotPrize = jackpotPrize
                                    });

                                }
                            catch (Exception) {

                               
                                }
                            }
                        }
                    }
                }
            }

        return lotteryResults;
        }
    }
namespace Application.Dtos.CurrencyRate;

public record ConvertResult(string Base, string Target, double Amount, double Result, double Rate, DateTime Date);
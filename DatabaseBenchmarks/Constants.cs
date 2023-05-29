namespace DatabaseBenchmarks;

public static class Constants
{
    public const int NumberOfIterations = 1000;
    public const int NumberOfWarmupIterations = 100;

    public static readonly string[] ProductCategories =
    {
        "Electronics", "Smartphones", "Laptops", "Cameras", "Headphones",
        "Home and Kitchen", "Furniture", "Cookware", "Lighting", "Kitchen appliances",
        "Sports", "Bicycles", "Swimming accessories", "Outdoor gear", "Exercise equipment",
        "Toys and Games", "Board games", "Puzzles", "Dolls", "Action figures",
        "Clothes", "Shirts", "Jeans", "Jackets", "Hoodies"
    };

    public static readonly string[] EuropeanCountries =
    {
        "Albania", "Andorra", "Austria", "Belarus", "Belgium", "Bosnia and Herzegovina",
        "Bulgaria", "Croatia", "Cyprus", "Czech Republic", "Denmark", "Estonia",
        "Finland", "France", "Germany", "Greece", "Hungary", "Iceland", "Ireland",
        "Italy", "Kosovo", "Latvia", "Liechtenstein", "Lithuania", "Luxembourg",
        "Malta", "Moldova", "Monaco", "Montenegro", "Netherlands",
        "Norway", "Poland", "Portugal", "Romania", "Russia", "San Marino",
        "Serbia", "Slovakia", "Slovenia", "Spain", "Sweden", "Switzerland",
        "Ukraine", "United Kingdom", "Vatican City"
    };

    public static readonly string[] PaymentMethods =
        {"Credit Card", "Debit Card", "PayPal", "Bank Transfer", "Cash on Delivery"};

    public static readonly string[] PaymentStatuses =
        {"Pending", "Authorized", "Charged", "Declined", "Refunded", "Canceled", "Error"};

    public static readonly string[] ShippingMethods =
        {"Pending", "Authorized", "Charged", "Declined", "Refunded", "Canceled", "Error"};
}
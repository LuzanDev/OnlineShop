namespace OnlineShop.Models.Enums
{
    public enum ErrorCodes
    {
        // Products 1 - 20
        ProductsNotFound = 1,

        // Brands 21 - 30
        BrandNotFound = 21,
        BrandCollectionNotFound = 22,
        BrandAlreadyExists = 23,

        // Category 31 - 40
        CategoryNotFound = 31,
        CategoryCollectionNotFound = 32,
        CategoryAlreadyExists = 33,
    }
}

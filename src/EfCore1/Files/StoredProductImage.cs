namespace EfCore1.Files;

public sealed record StoredProductImage
(
    string StoredFileName,
    long SizeInBytes,
    string ContentType
);
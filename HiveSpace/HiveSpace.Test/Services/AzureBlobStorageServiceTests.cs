using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HiveSpace.Application.Services;
using HiveSpace.Common.Exceptions;
using HiveSpace.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HiveSpace.Test.Services;

public class AzureBlobStorageServiceTests
{
    private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
    private readonly Mock<BlobContainerClient> _mockBlobContainerClient;
    private readonly Mock<BlobClient> _mockBlobClient;
    private readonly AzureBlobStorageService _blobStorageService;

    public AzureBlobStorageServiceTests()
    {
        _mockBlobServiceClient = new Mock<BlobServiceClient>();
        _mockBlobContainerClient = new Mock<BlobContainerClient>();
        _mockBlobClient = new Mock<BlobClient>();

        _mockBlobServiceClient
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_mockBlobContainerClient.Object);

        _mockBlobContainerClient
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_mockBlobClient.Object);

        _blobStorageService = new AzureBlobStorageService(_mockBlobServiceClient.Object);
    }

    [Fact]
    public async Task GetContainerClient_ShouldCreateContainerIfNotExists()
    {
        // Arrange

        // Act
        var containerClient = await _blobStorageService.GetContainerClient("test-container");

        // Assert
        Assert.Equal(containerClient, _mockBlobContainerClient.Object);
    }

    [Fact]
    public async Task DeleteFile_ShouldDeleteBlobIfExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockBlobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(true, null));

        // Act
        await _blobStorageService.DeleteFile(fileId, StorageType.Avatar);

        // Assert
        _mockBlobClient.Verify(x => x.DeleteAsync(default, default, default), Times.Once);
    }

    [Fact]
    public async Task DeleteFile_ShouldNotDeleteBlobIfNotExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockBlobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(false, null));

        // Act
        await _blobStorageService.DeleteFile(fileId, StorageType.CategoryImages);

        // Assert
        _mockBlobClient.Verify(x => x.DeleteAsync(default, default, default), Times.Never);
    }

    [Fact]
    public async Task GetFileUrl_ShouldReturnBlobUrlIfExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var expectedUrl = "https://example.com/blob";
        _mockBlobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(true, null));
        _mockBlobClient
            .SetupGet(x => x.Uri)
            .Returns(new Uri(expectedUrl));

        // Act
        var fileUrl = await _blobStorageService.GetFileUrl(fileId, StorageType.ProductImages);

        // Assert
        Assert.Equal(expectedUrl, fileUrl);
    }

    [Fact]
    public async Task GetFileUrl_ShouldThrowFileNotFoundExceptionIfBlobDoesNotExist()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockBlobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(false, null));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _blobStorageService.GetFileUrl(fileId, StorageType.Avatar));
    }

    [Fact]
    public async Task UploadFromFileAsync_ShouldUploadFileAndReturnFileId()
    {
        // Arrange
        var mockFormFile = new Mock<IFormFile>();
        var stream = new MemoryStream();
        mockFormFile.Setup(x => x.OpenReadStream()).Returns(stream);
        mockFormFile.Setup(x => x.Length).Returns(1024);
        mockFormFile.Setup(x => x.ContentType).Returns("image/png");

        _mockBlobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default));

        // Act
        var result = await _blobStorageService.UploadFromFileAsync(mockFormFile.Object, StorageType.Avatar);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockBlobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task UploadFromByteDataAsync_ShouldUploadDataAndReturnFileId()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4 };
        var contentType = "application/octet-stream";
        var storageType = StorageType.Avatar;

        _mockBlobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default));

        // Act
        var result = await _blobStorageService.UploadFromByteDataAsync(data, storageType, contentType);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockBlobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
    }

    [Fact]
    public void DeleteContainer_ShouldDeleteContainer()
    {
        // Arrange
        var storageType = StorageType.ProductImages;

        _mockBlobContainerClient
            .Setup(x => x.DeleteIfExists(default, default));

        // Act
        _blobStorageService.DeleteContainer(storageType);

        // Assert
        _mockBlobContainerClient.Verify(x => x.DeleteIfExists(default, default), Times.Once);
    }


}

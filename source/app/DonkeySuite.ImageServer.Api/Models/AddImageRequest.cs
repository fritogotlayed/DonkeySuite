namespace DonkeySuite.ImageServer.Api.Models
{
    public class AddImageRequest
    {
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        /// <value>The payload.</value>
        /// <remarks>This is a base64 encoded string of bytes.</remarks>
        public string Payload { get; set; }
    }
}
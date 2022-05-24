namespace API.DTO
{
    public class ReviewDTO
    {
        public UserDTO User { get; set; }
        public string Content { get; set; }
        public double Rating { get; set; }
    }
    public class CreateReviewDTO
    {
        public string Content { get; set; }
        public double Rating { get; set; }
    }
}

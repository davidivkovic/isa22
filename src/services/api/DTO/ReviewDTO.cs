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

    public class CreateComplaintDTO
    {
        public string Content { get; set; }
    }

    public class CreateReportDTO
    {
        public string Reason { get; set; }
        public bool Penalize { get; set; }

    }

    public class PendingReportDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public UserDTO User { get; set; }
        public BusinessDTO Business { get; set; }
        public string Reason { get; set; }
        public bool Penalize { get; set; }
    }

    public class PendingReviewDTO
    {
        public Guid Id { get; set; }
        public UserDTO User { get; set; }
        public BusinessDTO Business { get; set; }
        public double Rating { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
    }
}

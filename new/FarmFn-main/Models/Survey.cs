    using System.ComponentModel.DataAnnotations;

    namespace Farm.Models
    {
        public class Survey
        {
            public int Id { get; set; }

            // Thông tin cơ bản
            [Required(ErrorMessage = "Tên chuồng là bắt buộc.")]
            public string CageName { get; set; }

            public string CageCode { get; set; } // Không bắt buộc

            [Required(ErrorMessage = "Vị trí chuồng là bắt buộc.")]
            public string Location { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Diện tích chuồng phải lớn hơn hoặc bằng 0.")]
            public double CageArea { get; set; }

            // Thông tin chăn nuôi
            [Range(0, int.MaxValue, ErrorMessage = "Số lượng gà phải lớn hơn hoặc bằng 0.")]
            public int ChickenCount { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn giống gà nuôi.")]
            public string ChickenBreed { get; set; }

            public string BreedOther { get; set; } // Không bắt buộc

            [Required(ErrorMessage = "Vui lòng chọn mục đích nuôi.")]
            public string Purpose { get; set; }

            public string PurposeOther { get; set; } // Không bắt buộc

            [Required(ErrorMessage = "Thức ăn chính là bắt buộc.")]
            public string MainFeed { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn nguồn nước.")]
            public string WaterSource { get; set; }

            public string WaterSourceOther { get; set; } // Không bắt buộc

            // Điều kiện chuồng trại
            [Required(ErrorMessage = "Vui lòng chọn tình trạng thông gió.")]
            public string Ventilation { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn loại ánh sáng.")]
            public string Lighting { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn tình trạng vệ sinh.")]
            public string Hygiene { get; set; }

            // Ghi chú
            public string Notes { get; set; } // Không bắt buộc
        }
    }
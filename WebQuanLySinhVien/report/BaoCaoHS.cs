using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
namespace WebQuanLySinhVien.report
{
    public class BaoCaoHS : IDocument
    {
        private readonly BaoCaoVM _baocao;

        public BaoCaoHS(BaoCaoVM baocao)
        {
            _baocao = baocao;
        }

        //public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                // Header với quốc hiệu
                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                        .FontSize(12).Bold();

                    column.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc")
                        .FontSize(11).Italic();

                    column.Item().PaddingTop(10).AlignCenter().Text("BÁO CÁO TÌNH HÌNH HỌC TẬP")
                        .FontSize(14).Bold();
                });
                
                // Thông tin cá nhân
                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Item().Text($"Kính gửi: Ban giám hiệu nhà trường").FontSize(11);
                    column.Item().PaddingTop(10).Text($"Họ và tên người học: {_baocao.sinhvien.HoTen}").FontSize(11);
                    column.Item().Text($"Lớp/Khóa học: {_baocao.tenlop}").FontSize(11);
                    //column.Item().Text($"Khoa/Bộ môn: {studentInfo.Department}").FontSize(11);
                    column.Item().Text($"Giáo viên hướng dẫn: {_baocao.tenGV}").FontSize(11);
                    column.Item().Text($"Ngày báo cáo: {DateTime.Now.ToString("dd/MM/yyyy")}").FontSize(11);

                    // Mục I: Mục đích báo cáo
                    column.Item().PaddingTop(15).Text("I. Mục đích báo cáo").FontSize(12).Bold();
                    column.Item().PaddingVertical(5).Text("- Tóm tắt và đánh giá kết quả học tập trong suốt thời gian qua.").FontSize(11);
                    column.Item().Text("- Phân tích những điểm mạnh và yếu trong quá trình học.").FontSize(11);
                    column.Item().Text("- Đưa ra phương hướng cải thiện và phát triển trong giai đoạn học tiếp theo.").FontSize(11);

                    // Mục II: Đánh giá tổng quát
                    column.Item().PaddingTop(10).Text("II. Đánh giá tổng quát quá trình học tập")
                    .FontSize(12).Bold();
                    column.Item().PaddingVertical(5).Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("- Tóm tắt về tinh thần học tập:")
                                .FontSize(11);
                        });

                        row.RelativeItem().AlignLeft().AlignLeft().Text(new string('.', 129))
                            .FontSize(11)
                            .FontColor(Colors.Grey.Medium);
                    });

                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("- Ý thức chấp hành nội quy, kỷ luật của lớp học, trường học:")
                                .FontSize(11);
                        });

                        row.RelativeItem().AlignLeft().AlignLeft().Text(new string('.', 75))
                            .FontSize(11)
                            .FontColor(Colors.Grey.Medium);
                    });

                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("- Nhận xét về sự tiến bộ trong quá trình học tập (nếu có):")
                                .FontSize(11);
                        });

                        row.RelativeItem().AlignLeft().AlignLeft().Text(new string('.', 80))
                            .FontSize(11)
                            .FontColor(Colors.Grey.Medium);
                    });


                    // Mục III: Kết quả học tập
                    column.Item().PaddingTop(10).Text("III. Kết quả học tập").FontSize(12).Bold();

                    // Bảng điểm
                    column.Item().PaddingTop(5).Text("1. Kết quả các môn học").FontSize(11).Bold();
                    column.Item().Table(table =>
                    {
                        // Định nghĩa cột
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // STT
                            columns.RelativeColumn(2); // Môn học
                            columns.RelativeColumn(1.5f); // Điểm
                            columns.RelativeColumn(3); // Nhận xét
                        });

                        // Header bảng
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("STT").FontSize(10).Bold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("MÔN HỌC").FontSize(10).Bold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("ĐIỂM ĐẠT ĐƯỢC").FontSize(10).Bold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Text("NHẬN XÉT").FontSize(10).Bold();
                        });

                        // Dữ liệu bảng
                        int stt = 1;
                        foreach (var subject in _baocao.DSdiem)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Text(stt.ToString()).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Text(subject.MaHpNavigation.TenHp).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Text(subject.DiemHp.ToString()).FontSize(10);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Text("").FontSize(10);
                            stt += 1;
                        }
                    });

                    // Thành tích khác
                    column.Item().PaddingTop(10).Text("2. Điểm và số tín chỉ tích lũy").FontSize(11).Bold();
                        column.Item().Text($"- Điểm tích lũy: {_baocao.hoso.DiemTichLuy}").FontSize(11);
                        column.Item().Text($"- Số tín chỉ tích lũy: {_baocao.hoso.SoTctichLuy}").FontSize(11);
                });

                // Footer với số trang
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Trang ").FontSize(10);
                    text.CurrentPageNumber().FontSize(10);
                });
            });
        }
    }
}

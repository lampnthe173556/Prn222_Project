using EcormerProjectPRN222.Models.Vnpay;

namespace EcormerProjectPRN222.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}

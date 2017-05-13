namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    //<Response><Status>SUCCESS</Status><MemberId>4e5bbbe700a9ec11548966b9</MemberId>
    //<ActivationCode>A1KGC7</ActivationCode><SaleId>416827</SaleId></Response>
    public class EnrollDocument
    {
        public string MemberId { get; set; }

        public string ActivationCode { get; set; }

        public string SalesId { get; set; }

        public bool IsEnrolled { get; set; }
    }
}

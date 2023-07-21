using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SipayApi.Base;
using SipayApi.Data.Domain;
using SipayApi.Data.Repository;
using SipayApi.Schema;

namespace SipayApi.Service;



[ApiController]
[Route("sipy/api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository repository;
    private readonly IMapper mapper;
    public TransactionController(ITransactionRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public ApiResponse<List<TransactionResponse>> GetAll()
    {
        var entityList = repository.GetAll();
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }

    [HttpGet("{id}")]
    public ApiResponse<TransactionResponse> Get(int id)
    {
        var entity = repository.GetById(id);
        var mapped = mapper.Map<Transaction, TransactionResponse>(entity);
        return new ApiResponse<TransactionResponse>(mapped);
    }

    [HttpGet("GetByReference")]
    public ApiResponse<List<TransactionResponse>> GetByReference(string ReferenceNumber)
    {
        var entityList = repository.GetByReference(ReferenceNumber);
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }

    [HttpPost]
    public ApiResponse Post([FromBody] TransactionRequest request)
    {
        var entity = mapper.Map<TransactionRequest, Transaction>(request);
        repository.Insert(entity);
        repository.Save();
        return new ApiResponse();
    }

    [HttpGet("GetByParameter")]
    public ApiResponse<List<TransactionResponse>> GetByParameter([FromQuery] TransactionFilterModel filterModel)
    {
        // Sends the expressions generated with the filterModel.
        var entityList = repository.GetByParameter(entity =>
                                                   (entity.AccountNumber == filterModel.AccountNumber || filterModel.AccountNumber == null) &&
                                                   (entity.CreditAmount >= filterModel.MinAmountCredit || filterModel.MinAmountCredit == null) &&
                                                   (entity.CreditAmount <= filterModel.MaxAmountCredit || filterModel.MaxAmountCredit == null) &&
                                                   (entity.DebitAmount >= filterModel.MinAmountDebit || filterModel.MinAmountDebit == null) &&
                                                   (entity.DebitAmount <= filterModel.MaxAmountDebit || filterModel.MaxAmountDebit == null) &&
                                                   (entity.Description.Contains(filterModel.Description) || filterModel.Description == null) &&
                                                   (entity.TransactionDate >= filterModel.BeginDate || filterModel.BeginDate == null) &&
                                                   (entity.TransactionDate <= filterModel.EndDate || filterModel.EndDate == null) &&
                                                   (entity.ReferenceNumber == filterModel.ReferenceNumber || filterModel.ReferenceNumber == null)).ToList();

        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }
}

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
    public ApiResponse<List<TransactionResponse>> GetByParameter([FromQuery] int? accountNumber, [FromQuery] decimal? minAmountCredit, [FromQuery] decimal? maxAmountCredit, [FromQuery] decimal? minAmountDebit, [FromQuery] decimal? maxAmountDebit, [FromQuery] string? description, [FromQuery] DateTime? beginDate, [FromQuery] DateTime? endDate, [FromQuery] string? referenceNumber)
    {
        var entityList = repository.GetByParameter(entity => entity.AccountNumber == accountNumber || 
                                                   entity.CreditAmount >= minAmountCredit && entity.CreditAmount <= maxAmountCredit || 
                                                   entity.DebitAmount >= minAmountDebit && entity.DebitAmount <= maxAmountDebit || 
                                                   entity.Description.Contains(description) || 
                                                   entity.TransactionDate >= beginDate && entity.TransactionDate <= endDate || 
                                                   entity.ReferenceNumber == referenceNumber).ToList();

        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }
}

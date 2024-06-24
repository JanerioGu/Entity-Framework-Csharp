using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using loja.models;
using loja.services;
using loja.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<DepositoService>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<ServicoService>();
builder.Services.AddScoped<ContratoService>();

// Configuração do DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 37))));

// Configuração do JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

// Rotas para Autenticação
app.MapPost("/login", async (UserLogin userLogin, UserManager userManager) =>
{
    var token = await userManager.Authenticate(userLogin);
    if (token == null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(new { token });
});

// Proteger rotas com [Authorize] e JWT

// Produtos
app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
}).RequireAuthorization();

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
}).RequireAuthorization();

app.MapPost("/produtos", async (Produto produto, ProductService productService) =>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
}).RequireAuthorization();

app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
{
    if (id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch");
    }

    await productService.UpdateProductAsync(produto);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProductAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Clientes
app.MapGet("/clientes", async (ClienteService clienteService) =>
{
    var clientes = await clienteService.GetAllClientesAsync();
    return Results.Ok(clientes);
}).RequireAuthorization();

app.MapGet("/clientes/{id}", async (int id, ClienteService clienteService) =>
{
    var cliente = await clienteService.GetClienteByIdAsync(id);
    if (cliente == null)
    {
        return Results.NotFound($"Cliente with ID {id} not found.");
    }
    return Results.Ok(cliente);
}).RequireAuthorization();

app.MapPost("/clientes", async (Cliente cliente, ClienteService clienteService) =>
{
    await clienteService.AddClienteAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
}).RequireAuthorization();

app.MapPut("/clientes/{id}", async (int id, Cliente cliente, ClienteService clienteService) =>
{
    if (id != cliente.Id)
    {
        return Results.BadRequest("Cliente ID mismatch");
    }

    await clienteService.UpdateClientesync(cliente);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/clientes/{id}", async (int id, ClienteService clienteService) =>
{
    await clienteService.DeleteClienteAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Fornecedores
app.MapGet("/fornecedores", async (FornecedorService fornecedorService) =>
{
    var fornecedores = await fornecedorService.GetAllFornecedoresAsync();
    return Results.Ok(fornecedores);
}).RequireAuthorization();

app.MapGet("/fornecedor/{id}", async (int id, FornecedorService fornecedorService) =>
{
    var fornecedor = await fornecedorService.GetFornecedorByIdAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor with ID {id} not found.");
    }
    return Results.Ok(fornecedor);
}).RequireAuthorization();

app.MapPost("/fornecedor", async (Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    await fornecedorService.AddFornecedorAsync(fornecedor);
    return Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor);
}).RequireAuthorization();

app.MapPut("/fornecedor/{id}", async (int id, Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    if (id != fornecedor.Id)
    {
        return Results.BadRequest("Fornecedor ID mismatch");
    }

    await fornecedorService.UpdateFornecedorAsync(fornecedor);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/fornecedor/{id}", async (int id, FornecedorService fornecedorService) =>
{
    await fornecedorService.DeleteFornecedorAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Vendas
app.MapPost("/vendas", async (Venda venda, VendaService vendaService) =>
{
    try
    {
        await vendaService.AddVendaAsync(venda);
        return Results.Created($"/vendas/{venda.Id}", venda);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/detalhada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByProdutoIdAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/sumarizada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasSumarizadasByProdutoIdAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/detalhada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByClienteIdAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/sumarizada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasSumarizadasByClienteIdAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/depositos/{depositoId}/produtos", async (int depositoId, VendaService vendaService) =>
{
    var produtos = await vendaService.GetProdutosNoDepositoAsync(depositoId);
    return Results.Ok(produtos);
}).RequireAuthorization();

app.MapGet("/depositos/produto/{produtoId}", async (int produtoId, VendaService vendaService) =>
{
    var produto = await vendaService.GetQuantidadeProdutoNoDepositoAsync(produtoId);
    return Results.Ok(produto);
}).RequireAuthorization();

// Endpoint para adicionar estoque
app.MapPost("/depositos/{depositoId}/produtos/{produtoId}/adicionar", async (int depositoId, int produtoId, [FromBody] AdicionarEstoqueDto adicionarEstoqueDto, DepositoService depositoService) =>
{
    await depositoService.AddEstoqueAsync(depositoId, produtoId, adicionarEstoqueDto.Quantidade);
    return Results.Ok();
}).RequireAuthorization();

// Serviços
app.MapPost("/servicos", async (Servico servico, ServicoService servicoService) =>
{
    await servicoService.AddServicoAsync(servico);
    return Results.Created($"/servicos/{servico.Id}", servico);
}).RequireAuthorization();

app.MapPut("/servicos/{id}", async (int id, Servico servico, ServicoService servicoService) =>
{
    if (id != servico.Id)
    {
        return Results.BadRequest("Service ID mismatch");
    }

    await servicoService.UpdateServicoAsync(servico);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/servicos/{id}", async (int id, ServicoService servicoService) =>
{
    var servico = await servicoService.GetServicoByIdAsync(id);
    if (servico == null)
    {
        return Results.NotFound($"Service with ID {id} not found.");
    }
    return Results.Ok(servico);
}).RequireAuthorization();

// Contratos
app.MapPost("/contratos", async (Contrato contrato, ContratoService contratoService) =>
{
    await contratoService.AddContratoAsync(contrato);
    return Results.Created($"/contratos/{contrato.Id}", contrato);
}).RequireAuthorization();

app.MapGet("/clientes/{clienteId}/servicos", async (int clienteId, ContratoService contratoService) =>
{
    var servicos = await contratoService.GetServicosByClienteIdAsync(clienteId);
    return Results.Ok(servicos);
}).RequireAuthorization();



app.Run();

public record UserLogin(string Email, string Senha);

public class UserManager
{
    private readonly JwtSettings _jwtSettings;
    private readonly List<User> _users = new()
    {
        new User { Email = "janerio32@hotmail.com", Senha = "123456" }
    };

    public UserManager(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string?> Authenticate(UserLogin userLogin)
    {
        var user = _users.SingleOrDefault(u => u.Email == userLogin.Email && u.Senha == userLogin.Senha);
        if (user == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class JwtSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

public class User
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

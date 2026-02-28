using BarbeariaRocha.Modelos.Request.Usuario;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BarbeariaRocha.Modelos.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public required string Numero { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? Foto { get; set; }
        public required string Perfil { get; set; }
        public required string Senha { get; set; }
        public string? Agenda { get; set; }
        public bool Excluido { get; set; } = false;

        public Usuario() { }
        [SetsRequiredMembers]
        public Usuario(UsuarioCriarRequest request)
        {
            ValidarNumero(request.Numero);
            ValidarSenha(request.Senha);
            ValidarEmail(request.Email);

            Nome = request.Nome;
            Descricao = request.Descricao;
            Numero = request.Numero;
            Email = request.Email;
            Perfil = request.Perfil.ToString();
            Senha = request.Senha;
            Agenda = request.Agenda?.ToString();
        }

        public static void ValidarNumero(string numero)
        {
            if (numero.Length != 11 || !numero.All(char.IsDigit))
                throw new ArgumentException("O número deve conter exatamente 11 dígitos.", nameof(numero));
        }

        public static void ValidarSenha(string senha)
        {
            if (senha.Length < 6)
                throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.", nameof(senha));
        }

        public static void ValidarEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new ArgumentException("O email informado não é válido.", nameof(email));
        }
    }
}

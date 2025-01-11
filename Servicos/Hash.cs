using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Testezin.Servicos
{
    public class Hash
    {
        private HashAlgorithm _algoritmo;

        public Hash(HashAlgorithm algoritmo)
        {
            _algoritmo = algoritmo;
        }
        
        public string CriptografarSenha(string senha)
        {
          var ValorEncodado = Encoding.UTF8.GetBytes(senha);
          var senhaEncriptada = _algoritmo.ComputeHash(ValorEncodado);

          var stringBuilder = new StringBuilder();
          foreach (var caracter in senhaEncriptada)
          {
              stringBuilder.Append(caracter.ToString("X2"));
          }

          return stringBuilder.ToString();
        }

        public bool VerificarSenha(string senhaDigitada, string senhaCadastrada)
        {
          if (string.IsNullOrEmpty(senhaCadastrada)) throw new NullReferenceException("Cadastre uma senha.");

          var senhaEncriptada = _algoritmo.ComputeHash(Encoding.UTF8.GetBytes(senhaDigitada));

          var stringBuilder = new StringBuilder();
          foreach (var caractere in senhaEncriptada)
          {
              stringBuilder.Append(caractere.ToString("X2"));
          }

          return stringBuilder.ToString() == senhaCadastrada;
        }
    }
}
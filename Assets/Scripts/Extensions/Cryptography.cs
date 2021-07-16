using System.Security.Cryptography;
using System.Text;

public class Cryptography
{
    public static string GetSHA512(string text) 
    {
        SHA512 sha = new SHA512Managed();
 
        //compute hash from the bytes of text
        sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
   
        //get hash result after compute it
        byte[] result = sha.Hash;
 
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
          //change it into 2 hexadecimal digits
          //for each byte
          strBuilder.Append(result[i].ToString("x2"));
        }
     
      return strBuilder.ToString();
    }
}
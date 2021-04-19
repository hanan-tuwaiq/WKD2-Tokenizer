using System;

namespace WK2D2_Tokenizer
{
    class Program
    {

        public class Token 
        {
            public string type; // id | number
            public string value;
            public int position;
            public int lineNumber;
        }

        public abstract class Tokenizable
        {
            public abstract bool tokenizable(Tokenizer t);
            public abstract Token tokenize(Tokenizer t);
        }

        public class Tokenizer
        {
            public string input;
            public int currentPosition;
            public int lineNumber;

            public Tokenizer(string input)
            {
                this.input = input;
                this.currentPosition = -1;
                this.lineNumber = 1;
            }

            public char peek()
            {
                if (this.hasMore())
                {
                    return this.input[this.currentPosition + 1];
                }
                else
                {
                    return '\0';
                }
            }

            public char next()
            {
                char currentChar = this.input[++this.currentPosition];
                if (currentChar == '\n')
                {
                    this.lineNumber++;
                }

                return currentChar;
            }

            public bool hasMore()
            {
                return (this.currentPosition + 1) < this.input.Length;
            }

            public Token tokenize(Tokenizable[] handlers)
            {
                foreach (var t in handlers)//t=>a method
                {
                    if (t.tokenizable(this))//How does this work :/
                    {
                        return t.tokenize(this);
                    }
                }

                //throw new Exception("Unexpected token");
                return null;
            }

        }

        public class NumberTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)//method to check the conditions
            {
                return t.hasMore() && Char.IsDigit(t.peek());
            }

            public override Token tokenize(Tokenizer t)//method to return the full token
            {
                Token token = new Token();
                token.type = "number";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && Char.IsDigit(t.peek()))//if t isnt done AND the next char is a digit while loops
                {
                    token.value += t.next();
                }

                return token;
            }
        }

        public class WhiteSpaceTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsWhiteSpace(t.peek());
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "whitespace";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && Char.IsWhiteSpace(t.peek()))
                {
                    token.value += t.next();
                }

                return token;
            }
        }

        public class IdTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && (Char.IsLetter(t.peek()) || t.peek() == '_');
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "id";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && (Char.IsLetterOrDigit(t.peek()) || t.peek() == '_'))
                {
                    token.value += t.next();
                }

                return token;
            }
        }

        static void Main(string[] args)
        {
            /// After refactoring test
            string testCase = "78  a _PORT _ j u8 8u 500";//u8?
            Tokenizer t = new Tokenizer(testCase);
            Tokenizable[] handlers = new Tokenizable[] {
                  new NumberTokenizer()
                , new WhiteSpaceTokenizer()
                , new IdTokenizer()
            };
            Token token = t.tokenize(handlers);
            while (token != null)
            {
                Console.WriteLine("Token Value= " + token.value + " type= " + token.type);
                token = t.tokenize(handlers);
            }

        }
    }
}

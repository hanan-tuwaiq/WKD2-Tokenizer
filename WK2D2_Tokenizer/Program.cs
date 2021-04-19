using System;
using System.Collections;

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

            public char peek(int p = 1)
            {
                if (this.hasMore())
                {
                    return this.input[this.currentPosition + p];
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

            public bool hasMore(int p = 1)
            {
                return (this.currentPosition + p) < this.input.Length;
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
                if (t.peek() == '.')
                {
                    token.value += t.next();
                    while (t.hasMore() && Char.IsDigit(t.peek()))
                    {
                        token.value += t.next();
                    }
                    String[] str = token.value.Split(".");
                    if (str[1].Length < 8)
                    {
                        token.type = "Float";
                    }
                    else
                    {
                        token.type = "Double";
                    }
                }
                return token;
            }
        }

        public class WhiteSpaceTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsWhiteSpace(t.peek()) && t.peek() != '\n';
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
            public bool isKeyword(string modifer)
            {
                string[] keywords = { "public", "private", "protected", "internal","true", "false"
                ,"abstract" ,"async", "const", "event", "extern", "new", "override", "partial"};//list not complete
                int flag = 0;
                foreach (var key in keywords)
                {
                    if (modifer == key)
                    {
                        flag = 1;
                    }

                }
                return flag == 1;
            }
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
                if (isKeyword(token.value))
                {
                    token.type = "keyword";
                }
                return token;
            }
        }
        //one line comment
        public class OneLineCommentTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                if (t.hasMore() && t.peek() == '/')
                {
                    //Console.WriteLine("first slash");
                    if (t.hasMore() && t.peek(2) == '/')
                    {
                        //Console.WriteLine("second slash");
                        return true;
                    }
                }
                return false;
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "one line comment";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore() && t.peek() != '\n')
                {
                    token.value += t.next();
                }
                return token;
            }
        }
        //multi line comment
        public class MultiLineCommentTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                if (t.hasMore() && t.peek() == '/')
                {
                    if (t.hasMore(2) && t.peek(2) == '*')
                    {
                        return true;
                    }
                }
                return false;
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "Mutli Line Comment";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore())
                {
                    if (t.hasMore() && t.peek() == '*')
                    {
                        if (t.hasMore() && t.peek(2) == '/')
                        {
                            //t.next();
                            token.value += t.next();
                            token.value += t.next();
                            break;
                        }
                        token.value += t.next();
                    }
                    token.value += t.next();
                }
                return token;
            }
        }
        //operators
        public class OperatorTokenizer : Tokenizable
        {
            public bool isOperator(char x)
            {
                if (x == '=' || x == '+' || x == '-' || x == '%' || x == '*' || x == '<' || x == '>')
                    return true;
                return false;
            }
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && (isOperator(t.peek()));
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "Operator";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore() && (isOperator(t.peek())))
                {
                    token.value += t.next();
                }
                return token;
            }
        }
        //Punctuation
        public class PunctuationTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsPunctuation(t.peek());
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "Punctuation";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore() && Char.IsPunctuation(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }
        //lowercase
        public class LowerCasesTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsLetter(t.peek()) && Char.IsLower(t.peek());
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "Lowercase";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore() && Char.IsLetter(t.peek()) && Char.IsLower(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }
        //uppercase
        public class UpperCasesTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsLetter(t.peek()) && Char.IsUpper(t.peek());
            }
            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.type = "Uppercase";
                token.value = "";
                token.position = t.currentPosition;
                token.lineNumber = t.lineNumber;
                while (t.hasMore() && Char.IsLetter(t.peek()) && Char.IsUpper(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }
        static void Main(string[] args)
            {
                Console.WriteLine("===============");
                string testCase = "78 true _PORT ! . 45.67 78.88888888 4566 /*fff*/ ++ = TEA tea //hhhh  5 ";
                Tokenizer t = new Tokenizer(testCase);
                Tokenizable[] handlers = new Tokenizable[] {
                  new OneLineCommentTokenizer()
                , new WhiteSpaceTokenizer()
                , new OperatorTokenizer()
                , new NumberTokenizer()
                , new IdTokenizer()
                , new MultiLineCommentTokenizer()
                , new PunctuationTokenizer()
                , new UpperCasesTokenizer()
                , new LowerCasesTokenizer()
            };
                Token token = t.tokenize(handlers);
                while (token != null)
                {
                    Console.WriteLine(token.value + " : " + token.type);
                    token = t.tokenize(handlers);
                }

            }
        }
    }

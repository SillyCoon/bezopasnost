using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EncryctionDES
{
    class DES
    {
        #region Useful Tables

        private int[] FinalReshuffle =
        {
            40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,
    38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,
    36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,
    34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25
        };

        private int[] StartingReshuffle =
        {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
        };

        private int[] KeyReshuffle = {
            57,  49,  41,  33,  25,  17,  9,   1,   58,  50,  42,  34,  26,  18,
            10, 2,   59,  51,  43,  35,  27,  19,  11,  3,   60,  52,  44,  36,
            63,  55,  47,  39,  31,  23,  15,  7,   62,  54,  46,  38,  30,  22,
            14,  6,   61,  53,  45,  37,  29,  21,  13,  5,   28,  20,  12,  4
        };

        private int[] ForEFunction =
        {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,1
        };

        private int[] ForKi =
        {
            14,  17,  11,  24,  1 ,  5 ,  3 ,  28 , 15 , 6  , 21 , 10 , 23 , 19 , 12,  4,
            26,  8,  16,  7,   27,  20,  13,  2,   41,  52,  31,  37,  47,  55,  30,  40,
            51,  45,  33,  48,  44,  49,  39,  56,  34,  53,  46,  42,  50,  36,  29,  32
        };

        private int[] ForP ={
    16,7,20,21,29,12,28,17,
    1,15,23,26,5,18,31,10,
    2,8,24,14,32,27,3,9,
    19,13,30,6,22,11,4,25
};

        #endregion;

        List<int[,]> S = new List<int[,]>();

        private const int BlockSize = 64;
        private const int CharSize = 8;
        private const int KeySize = 56;
        private const int EConst = 48;

        private string[] Blocks;

        private string str; // шифруемый текст
        private string key; // ключ

        public DES(string str, string key)
        {
            this.str = str;
            this.key = key;

            CompleteString();
            CorrectKey();
            DivideTextIntoBlocks();

            this.key = StringToBinaryFormat(key);
            this.key = new string(KeyReshuffleFunc(this.key.ToCharArray()));

            

            #region Tables S

         int[,] S1 =

        {
            { 14, 4,   13,  1,   2,   15,  11,  8,  3,   10,  6 ,  12,  5,   9,   0,   7 },
            { 0,   15,  7,   4,   14  ,2   ,13  ,1   ,10  ,6   ,12  ,11  ,9   ,5   ,3   ,8 },
            { 4   ,1   ,14  ,8   ,13  ,6   ,2   ,11  ,15  ,12  ,9   ,7   ,3   ,10  ,5   ,0 },
            { 15  ,12  ,8   ,2   ,4   ,9   ,1   ,7   ,5   ,11  ,3   ,14  ,10  ,0   ,6   ,13}
        };

         int[,] S2 =
        {
    {15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10,},
    {3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5},
    {0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15,},
    {13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9}
};

         int[,] S3 ={
    {10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8,},
    {13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1},
    {13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7,},
    {1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12}
};

         int[,] S4 = {
    {7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15,},
    {13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9},
    {10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4,},
    {3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14}
};
         int[,] S5 = {
    {2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9,},
    {14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6},
    {4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14,},
    {11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3,}
};
         int[,] S6 = {
    {12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11,},
    {10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8},
    {9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6,},
    {4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13}
};
         int[,] S7 ={
    {4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1,},
    {13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6},
    {1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2,},
    {6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12,}
};
         int[,] S8 = {
    {13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7,},
    {1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2},
    {7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8,},
    {2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11,}
};
            #endregion;

            #region List S
            S.Add(S1);
            S.Add(S2);
            S.Add(S3);
            S.Add(S4);

            S.Add(S5);
            S.Add(S6);
            S.Add(S7);
            S.Add(S8);

            #endregion;
        }

        // Дополняем строку * до кратности размеру блока (64)
        private void CompleteString()
        {
            while (str.Length * CharSize % BlockSize != 0)
            {
                str += '*';
            }
        }

        // Разбиваем строку на блоки по 64 бита

        private void DivideTextIntoBlocks()
        {
            Blocks = new string[str.Length * CharSize / BlockSize];

            int blockLength = str.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = str.Substring(i * blockLength, blockLength);
                Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }

        // Переводит строку в двоичный формат
        private string StringToBinaryFormat(string str)
        {
            string binaryString = "";

            for (int i = 0; i < str.Length; i++)
            {
                string binary = Convert.ToString(str[i], 2); // Переводим в двоичный формат каждый символ строки

                // Дополняем незначащими нулями до длины байта
                while (binary.Length < CharSize)
                {
                    binary = "0" + binary;
                }
                binaryString += binary;
            }

            return binaryString;
        }

        // Разбивает двоичную строку на блоки
        private void DivideBinaryTextIntoBlocks(string binaryText) {
            Blocks = new string[binaryText.Length / BlockSize];
            int BlockLength = binaryText.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = binaryText.Substring(i * BlockLength, BlockLength);
            }
        }

        // Преобразуем ключ до нужной длины
        private void CorrectKey()
        {
            if (key.Length > KeySize/8)
            {
                key = key.Substring(0, KeySize/8);
            }
            else
            {
                while (key.Length < KeySize/8)
                {
                    key += "0";
                }
            }
        }

        // Меняем биты в блоке по схеме (см. вики)
        // получаем IP(T)
        /*public char[,] FirsStep()
        {
            char[,] changedBlocks = new char [Blocks.Length, BlockSize]; // Преобразованный блок

            // Проходим по каждому блоку и меняем местами биты по схеме
            for (int j = 0; j < Blocks.Length; j++)
            {
                for (int i = 0; i < BlockSize; i++)
                {
                    changedBlocks[j, i] = Blocks[j][StartingReshuffle[i] - 1];
                }
            }

            return changedBlocks;
        }*/

        // Первоначальная перестановка, получаем IP_T
        private string GetIP_T(string IP)
        {
            string IP_T = "";

            for (int i = 0; i < BlockSize; i++)
            {
                IP_T += IP[StartingReshuffle[i] - 1];
            }

            return IP_T;
        }

        // 16 великих преобразований
        public string Encryct()
        {
            string result = "";
            // Проходим по каждому из имеющихся блоков
            for (int j = 0; j < Blocks.Length; j++)
            {
                // Производим первоначальную перестановку блока
                string IP_T = GetIP_T(Blocks[j]);
                
                // Разбиваем блок на 2 равные части
                // Получаем первые значения (пригодятся)
                string RiPrev = IP_T.Substring(32, 32);
                string LiPrev = IP_T.Substring(0, 32);
                string KiPrev = key;

                // Применяем основную функцию шифрования (Фейстеля) 16 раз
                for (byte i = 0; i < 16; i++)
                {
                    string ki = getKi(i+1);
                    string Li = RiPrev;
                    char[] RiChar = new char[32];


                    string feistelResult = FeistelFunction(RiPrev, ki);

                    for (int y = 0; y < 32; y++)
                    {
                        byte t1 = (byte)Char.GetNumericValue(LiPrev[y]);
                        byte t2 = (byte)Char.GetNumericValue(feistelResult[y]);
                        RiChar[y] = Convert.ToString((t1 ^ t2), 2)[0];
                    }

                    string Ri = new string(RiChar);
                    
                    RiPrev = Ri;
                    KiPrev = ki;
                    LiPrev = Li;
                }

                // Получаем итоговый ответ
                string T16 = LiPrev + RiPrev;
                string oneBlock = "";
                for (int i = 0; i < BlockSize; i++)
                {
                    oneBlock += T16[FinalReshuffle[i]-1];
                }
                result += oneBlock;
            }
            return result;
        }

        // Функция Фейстеля
        public string FeistelFunction(string R, string k)
        {
            char[] E = EFunction(R);
            byte[] B = new byte[48];

            // Побитовое сложение ключа и преобразованной правой части
            for (int i = 0; i < 48; i++)
            {
                byte t1 = (byte)Char.GetNumericValue(k[i]);
                byte t2 = (byte)Char.GetNumericValue(E[i]);
                B[i] = (byte)(t1 ^ t2);
            }

            // Проводим хитрые преобразования с исходным словом B (см. вики)
            // В результате получаем B' - измененную строку
            string BChangedBinaryString = "";

            for (int i = 0; i < 8; i++)
            {
                int a = ToBinaryInt(String.Concat(B[6 * i], B[5 + (6 * i)]));
                int b = ToBinaryInt(String.Concat(B[1 + (6 * i)], B[2 + (6 * i)], B[3 + (6 * i)], B[4 + (6 * i)]));
                BChangedBinaryString += ToFourBitWord(S[i][a, b]);
            }

            char[] PB = PFunction(BChangedBinaryString);

            return new string(PB);
        }
        
        // P-функция перестановки в B'
        // По ней получаем результат Фейстеля
        private char[] PFunction(string Bchanged)
        {
            char[] PB = new char[32];

            for (int i = 0; i < 32; i++)
            {
                PB[i] = Bchanged[ForEFunction[i] - 1];
            }

            return PB;
        }

        // E-функция, дополняет R до 48 битов
        // Получаем ER
        private char[] EFunction(string R)
        {
            char[] ER = new char [EConst];

            for(int j = 0; j < EConst; j++)
            {
                ER[j] = R[ForEFunction[j] - 1];
            }

            return ER;
        }

        // Первичная перестановка ключа
        private char [] KeyReshuffleFunc(char [] K)
        {
            char[] changedKey = new char[BlockSize];
            char[] replacedKey = new char[KeySize];

            int c = 0;
            int j = 0;
            // Дополняем ключ до 64-битного
            for (int i = 0; i < BlockSize; i++)
            {
                if (i % 7 == 0 && i !=0 )
                {
                    if (c % 2 == 0)
                    {
                        changedKey[i] = '1';
                    }
                    else
                    {
                        changedKey[i] = '0';
                    }
                    c = 0;
                    continue;
                }
                c += (int)Char.GetNumericValue(K[j]);
                changedKey[i] = K[j];
                j++;
            }

            //Выполняем перестановку согласно схеме
            for (int i = 0; i < KeySize; i++)
            {
                replacedKey[i] = changedKey[KeyReshuffle[i] - 1];
            }

            return replacedKey;
        }
        
        // Получаем ключ для каждого шага
        private string getKi (int step)
        {
            string previousK = this.key;
            char[] Ki = new char[48];

            // Делим ключ на равные части
            char[] C0 = previousK.Substring(0, 28).ToCharArray();
            char[] D0 = previousK.Substring(28, 28).ToCharArray();

            // Циклический сдвиг влево в зависимости от условия step
            char[] Ci = LeftShift(C0);
            char[] Di = LeftShift(D0);

            if (step != 1 && step != 2 && step != 9 && step != 16)
            {
                Ci = LeftShift(Ci);
                Di = LeftShift(Di);
            }

            // Объединяем Ci и Di
            char[] CiDi = new char[56];
            Ci.CopyTo(CiDi, 0);
            Di.CopyTo(CiDi, Ci.Length);

            // Заполняем Ki
            for (int i = 0; i<48; i++)
            {
                Ki[i] = CiDi[ForKi[i]-1];
            }

            return new string(Ki);
        }

        #region Вспомогательные функции
        // Циклический сдвиг влево
        private char[] LeftShift(char[] array)
        {
            char tempForEnd = array[0];

            for (int i = 1; i<28; i++)
            {
                array[i-1] = array[i];
            }
            array[27] = tempForEnd;

            return array;
        }

        // Превращаем бинарную строку в int
        private int ToBinaryInt(string str)
        {
            int res = Convert.ToInt32(str, 2);
            return res;
        }

        // Превращаем число в 4-битное двоичное слово
        private string ToFourBitWord(int num)
        {
            return Convert.ToString(num, 2).PadLeft(4, '0');
        }

        #endregion
    }
}

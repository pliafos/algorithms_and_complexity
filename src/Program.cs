using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class Alco
{   

     List<int> Lessons = new List<int>();
     List<List<int>> Exam_Students;
     List<List<int>> Student_Exams = new List<List<int>>();
     int Conflicts_Count;
     int Conflicts_Unique_Count;
     List<int> Conflict_Lines_Arg;

     int[,] Lessons_Matrix;
     double calc=0;
     int registrations = 0;

     int Students = 0;
    public void init()
    { 
        FileOpen("datasets/hec-s-92.stu");
        Lessons.Sort();
        Exam_Students = new List<List<int>>(Lessons.Count);

        for(int i = 0; i < Lessons.Count; i ++)
        {
            Exam_Students.Add(new List<int>());
        }
        Initialize_Exam_Students();
       
        Conflicts_Count = 0;
        Conflicts_Unique_Count = 0;
        Conflict_Lines_Arg = new List<int>(new int[Lessons.Count]);
        Adjc();

        
        
        for(int i = 0; i < Lessons.Count; i ++)
        {
            for (int j = 0; j < Lessons.Count; j++)
            {
                
               // Console.WriteLine ("[x:"+(i)+ "," + " " + "y:"+(j)+"] :" + " " + Lessons_Matrix[i,j]);
            }
           // Console.WriteLine ("Arguments : " + Conflict_Lines_Arg[i]);
        }

        
        double density = (double) Conflicts_Unique_Count / ((double)(Lessons.Count*Lessons.Count));
        Console.WriteLine( "Lessons : " + Lessons.Count);
        Console.WriteLine( "Students : " + Student_Exams.Count);
        Console.WriteLine( "Registrations : " + registrations);
        Console.WriteLine( "All Conficts : " + Conflicts_Count);
        Console.WriteLine( "Density : " + density);
        Console.WriteLine( "Max : " + Find_Max());
        Console.WriteLine( "Min : " + Find_Min());
        Console.WriteLine( "Mean : " + Find_Mean());
        Console.WriteLine( "Med : " + Find_Med());
        Console.WriteLine( "CV : " + Find_CV());
        Console.WriteLine( "Max Colors : " + First_Fit());
    }
    
    public int Find_Max()
    {
        int matrixMax = 0;
        for(int i = 0; i < Lessons.Count; i++)
        {
            int tempMax = 0;
            for(int j = 0; j < Lessons.Count; j++)
            {
                if (Lessons_Matrix[i,j] > 0)
                    tempMax++;
            }
            if (tempMax > matrixMax)
            {
                matrixMax = tempMax;
            }
        }
        return matrixMax;
    }

public int Find_Min()
    {
        int matrixMin = Find_Max();
        for(int i = 0; i < Lessons.Count; i++)
        {
            int tempMin = 0;
            for(int j = 0; j < Lessons.Count; j++)
            {
                if (Lessons_Matrix[i,j] > 0)
                    tempMin++;
            }
            if (tempMin < matrixMin)
            {
                matrixMin = tempMin;
            }
        }
        return matrixMin;
    }

public double Find_Mean()
{
    return (double)((double)Conflicts_Unique_Count/(double)Lessons.Count);
}

public double Find_Med()
{
    List<int> sorted = new List<int>(); 
    for (int i = 0; i < Lessons.Count; i++)
        {
            int c =0;
            for  (int j = 0; j < Lessons.Count; j++)
            {    
                if (Lessons_Matrix[i,j] > 0)
                c++;           
            }
            sorted.Add(c);
        }
    sorted.Sort();
    return sorted[sorted.Count/2+(sorted.Count % 2 == 0 ? 0 : 1)];
}

public double Find_CV()
{

    for (int i = 0; i < Lessons.Count; i++)
        {
            int c =0;
            for  (int j = 0; j < Lessons.Count; j++)
            {   
                if (Lessons_Matrix[i,j] > 0)
                    c++;
            }
            calc += (double)Math.Pow((c - Find_Mean()),2);
        }

        calc = (calc/(double)(Lessons.Count));
        return (Math.Sqrt(calc) / Find_Mean()) *100;

}

    public  void Adjc()
    {
        Lessons_Matrix = new int[Lessons.Count,Lessons.Count];
        for (int i = 0; i < Lessons.Count; i++)
        {
            for  (int j = 0; j < Lessons.Count; j++)
            {
                if (i == j)
                {
                    Lessons_Matrix[i,j] = 0;
                    continue;
                }
                Lessons_Matrix[i,j] = findColisions(Exam_Students[i] , Exam_Students[j]);

                if (Lessons_Matrix[i,j] > 0)
                {
                    Conflict_Lines_Arg[i] += Lessons_Matrix[i,j];
                    Conflicts_Count += Lessons_Matrix[i,j];
                    Conflicts_Unique_Count++;
                }
            }
        }
    }

    public  int findColisions(List<int> ls1 , List<int> ls2)
    {
        int col = 0;
        for (int i = 0; i < ls1.Count; i++)
        {
            for  (int j = 0; j < ls2.Count; j++)
            {
                if(ls1[i] == ls2[j])
                {
                    col++;
                    break;
                }                    
            }
        }
        return col;
    }

    public  bool FileOpen(string path)
    {
        try
        {
            StreamReader sr = new StreamReader(path);

            string line;

            line = sr.ReadLine(); 
            while (line != null) 
            {
                Students++;
                string[] frag = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                List<int> temp = new List<int>(); 
                for (int i = 0; i < frag.Length; i++)
                {
                    temp.Add(int.Parse(frag[i]));
                    Store_Unique(frag[i]);
                }
                Student_Exams.Add(temp); 
                line = sr.ReadLine(); 
            }
        }
        catch (Exception ex)
        {
            string msg = ex.Message;
            Console.WriteLine("Could not open file : \n" + msg);
            Environment.Exit(0);
            return false;

        }
        return true;
        
    }

    public  void Initialize_Exam_Students()
    {
        for (int i = 0; i < Student_Exams.Count; i++)
        {
            for (int j = 0; j < Student_Exams[i].Count; j++)
            {
                registrations++;
                Exam_Students[Student_Exams[i][j]-1].Add(i);
            }
        }
    }

    public  void Store_Unique(string lessonId)
    {
        if (Lessons.Contains(int.Parse(lessonId)))
        return;

        Lessons.Add(int.Parse(lessonId));
    }

    public bool Is_Vertex_Connected(int lesson1 , int lesson2)
    {
        for(int i=0;i < Exam_Students[lesson1].Count; i ++)
        {
            for (int j=0;j < Exam_Students[lesson2].Count; j++)
            {
                if (Exam_Students[lesson1][i] == Exam_Students[lesson2][j])
                    return true;
            }
        }
        return false;
    }

    public int First_Fit()
    {
        int max_colors=0;
        int[] Lesson_Color = new int[Lessons.Count];

        for (int i = 0; i < Lessons.Count; i++)
        {
            Lesson_Color[i] = 0;
        }

        for (int i = 0; i < Lessons.Count; i++)
        {
            int curColor = 1;
            bool colorFound = false;
            while(colorFound==false)
            {
                colorFound = true;
                for (int j = 0; j < Lessons.Count; j++)
                {
                    if (i !=j && Is_Vertex_Connected(i , j))
                    {
                        if (curColor == Lesson_Color[j])
                        {
                            curColor++;
                            colorFound = false;
                            break;
                        }
                    }
                }

            }
            if (curColor > max_colors)
                max_colors = curColor;

            Lesson_Color[i] = curColor;
            
        }

        return max_colors;

    }

}



class Program
{
    
    static void Main(string[] args)
    {    
        Alco al = new Alco();
        al.init();
    }

}


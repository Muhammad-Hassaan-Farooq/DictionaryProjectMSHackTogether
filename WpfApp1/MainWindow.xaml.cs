using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Reader r = new Reader();
        public MainWindow()
        {
            r.read();
            InitializeComponent();
            


        }

        private void clickcheck(object sender, RoutedEventArgs e)
        {
            String text = textbox1.Text;
            text = text.Trim();
            text.ToLower();
            List<Suggestion> corrections = r.autoCorrect(text);
            String ans = "";
            if (corrections == null)
            {
                ans = "No corrections";
            }
            else
            {
                for (int i = 0; i < corrections.Count; i++)
                {
                    Suggestion suggestion = corrections[i];
                    ans = ans + suggestion.word;
                    ans = ans + "\n";
                }
            }
            correctionbox.Text=ans;
            List<String> completions = r.autoComplete(text);
            ans = "";
            int count = 0;

            if (completions == null)
            {
                ans = "No suggestions";
            }
            else
            {
                for (int i = 0; i < completions.Count; i++)
                {
                    String suggestion = completions[i];
                    ans = ans + suggestion;
                    ans = ans + "\n";
                    count++;
                    if (count == 9)
                    {
                        break;
                    }
                }
            }
            suggestionbox.Text=ans;
        }

        private void clickadd(object sender, RoutedEventArgs e)
        {
            r.insert(textbox1.Text.Trim());
        }
        private void textbox1_TextChanged(object sender, TextChangedEventArgs e) { }
    }
        









    public class TrieNode {
        public TrieNode[] children;
        public Boolean wordComplete;
        public int wordCount;

        public TrieNode()
        {
            children = new TrieNode[26];
            wordCount = 0;
            wordComplete = false;
        }
    }




    public class Trie {
        TrieNode root;

        public Trie() {
            root = new TrieNode();
        }

        public void insert(String key)
        {
            TrieNode temp = root;
            key = key.ToLower();
            for (int i = 0; i < key.Length; i++)
            {
                int index = key[i] - 'a';
                if (index <= 26 && index >= 0)
                {
                    if (temp.children[index] == null)
                    {
                        temp.children[index] = new TrieNode();
                    }
                    temp = temp.children[index];
                    temp.wordCount++;
                }

            }
            temp.wordComplete = true;
        }

        public Boolean search(String key)
        {
            TrieNode temp = root;
            key = key.ToLower();
            for (int i = 0; i < key.Length; i++)
            {
                int index = key[i] - 'a';
                if (temp.children[index] == null)
                {
                    return false;
                }
                temp = temp.children[index];
            }
            if (temp.wordComplete == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<String> autoComplete(String key)
        {
            if (key.Trim().Length == 1) {
                return null;
                }
            TrieNode temp = root;
            key = key.ToLower();
            for (int i = 0; i < key.Length; i++)
            {
                int index = key[i] - 'a';
                if (temp.children[index] == null)
                {

                    return null;
                }
                temp = temp.children[index];
            }
            List<String> suggestions = new List<String>();
            suggestions = getSuggestions(temp, key, suggestions);
            if (suggestions.Count == 0)
            {

                return null;
            }
            else
            {
                return suggestions;
            }
        }

        public List<String> getSuggestions(TrieNode temp, String key, List<String> suggestions)
        {
            if (temp.wordComplete == true)
            {
                suggestions.Add(key);
            }
            for (int i = 0; i < 26; i++)
            {
                if (temp.children[i] != null)
                {
                    getSuggestions(temp.children[i], key + (char)(i + 'a'), suggestions);
                }
            }
            return suggestions;
        }

        public int levenshteinDistance(String key1, String key2)
        {
            int[,] dp = new int[key1.Length + 1,key2.Length + 1];

            for (int i = 0; i <= key1.Length; i++)
            {
                for (int j = 0; j <= key2.Length; j++)
                {

                    if (i == 0)
                    {
                        dp[i,j] = j;
                    }

                    else if (j == 0)
                    {
                        dp[i,j] = i;
                    }

                    else
                    {

                        dp[i,j] = minm_edits(dp[i - 1,j - 1]
                                + NumOfReplacement(key1[i - 1], key2[j - 1]),
                                dp[i - 1,j] + 1,
                                dp[i,j - 1] + 1);
                    }
                }

            }
            return dp[key1.Length,key2.Length];
        }

        private int NumOfReplacement(char c1, char c2)
        {
            if (c1 == c2)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }

        private int minm_edits(params int[] nums)
        {
            try {
                return nums.Min();
            }
            catch (Exception e) {
                return Int32.MaxValue;
            }
        }

    }
    




    public class Reader {
        String filename = "Dictionary.txt";
        String shuffleFile = "Shuffled.txt";
        Trie tree;
        
         public Reader() {
        tree = new Trie();
    }

    public void read() {

        try {
            StreamReader br = new StreamReader("Dictionary.txt");
            String word;
            while (!br.EndOfStream) {
                    word = br.ReadLine();
                tree.insert(word.Trim());
            }
            br.Close();
        } catch (Exception e) {
            Console.WriteLine("Exception");
        }

    }

    public void insert(String key) {
        tree.insert(key);
        try {
            StreamWriter fw = new StreamWriter("Dictionary.txt", true);
            fw.Write("\n");
            fw.Write(key);
            fw.Close();
        } catch (IOException e) {
        }

    }

    public Boolean search(String key) {
        if (tree.search(key)) {
            return true;
        } else {
            return false;
        }
    }

    public List<String> autoComplete(String key) {
        List<String> suggestions = tree.autoComplete(key);

        return suggestions;
    }

    public List<Suggestion> autoCorrect(String key) {

            if (key.Trim().Length == 1)
            {
                return null;
            }
            List<Suggestion> correction = new List<Suggestion>();
        if (!tree.search(key)) {
            for (int i = key.Length / 2; i < key.Length; i++) {
                String subString = key.Substring(0, i);
                List<String> suggest = tree.autoComplete(subString);
                if (suggest != null) {
                    for (int j = 0; j < suggest.Count; j++) {
                        String word = suggest[j];
                        correction.Add(new Suggestion(word, tree.levenshteinDistance(key, word)));
                    }
                }

            }
        }
        bubbleSort(correction, key);

        List<Suggestion> Final = new List<Suggestion>();
        foreach (Suggestion suggestion in correction) {
            if (suggestion.distance < 3) {
                Final.Add(suggestion);
                // System.out.println(suggestion.word);
            }
            if (Final.Count == 9) {
                break;
            }
        }
        if (Final.Count == 0) {
            return null;
        }
        return Final;
    }

    public void bubbleSort(List<Suggestion> corrections, String key) {
        for (int i = 0; i < corrections.Count - 1; i++) {
            for (int j = 0; j < corrections.Count - i - 1; j++) {
                Suggestion str1 = corrections[j];
                Suggestion str2 = corrections[j + 1];
                if (str2.distance < str1.distance) {
                    corrections[j]=str2;
                    corrections[j + 1]=str1;
                }

            }

        }
    }

    public static void main(String[] args) {
        Reader r = new Reader();
        r.read();
        Console.Write(r.autoCorrect("correc"));
            
    }
    }





    public class Suggestion
    {
    public String toString()
        {
            return word;
        }

        public String word;
        public int distance;

        public Suggestion(String word, int distance)
        {
            this.word = word;
            this.distance = distance;
        }

    }
}

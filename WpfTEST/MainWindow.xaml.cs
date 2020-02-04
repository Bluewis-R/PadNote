using System;
using System.Collections.Generic;
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

//using CalcMaths;
using System.IO;

namespace WpfTEST
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      






    }

    private void textChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox currentTextBox = sender as TextBox;

      Grid targetGrid = UIHelper.FindVisualParent<Grid>(currentTextBox);
      Button targetButton = UIHelper.FindChild<Button>(targetGrid, "OutputBox");

      targetButton.Content = currentTextBox.Text;



      string tex;
      var ctx = new MyFunctionContext();
      //lib
      try
      {
        // need to set up library

        tex = Parser.Parse(currentTextBox.Text).Eval(ctx).ToString();
      }
      catch (Exception)
      {
        tex = "error";
      }

      targetButton.Content = tex;
    }


    private void NewLineButton_Click(object sender, RoutedEventArgs e)
    {
      //  Making grid deffinitions
      Grid newLine = new Grid();
      //newline.Name = 

      ColumnDefinition c1 = new ColumnDefinition();
      ColumnDefinition c2 = new ColumnDefinition();
      c1.Width = new GridLength(5, GridUnitType.Star);
      c2.Width = new GridLength(1, GridUnitType.Star);
      newLine.ColumnDefinitions.Add(c1);
      newLine.ColumnDefinitions.Add(c2);

      //  Making elements
      newLine.Margin = new Thickness(0, 0, 0, 5);
      StackPanel tempSP = new StackPanel(); 
      

      TextBox tempTB = new TextBox();
      tempTB.Padding = new Thickness(0, 5, 0, 5);
      tempTB.TextChanged += textChangedEventHandler;
      Grid.SetColumn(tempTB, 0);

      Button tempBttn = new Button();
      tempBttn.Padding = new Thickness(0, 5, 0, 5);
      tempBttn.Margin = new Thickness(10, 0, 0, 0);
      tempBttn.Name = "OutputBox";
      Grid.SetColumn(tempBttn, 1); 

      //  Adding everything to the Xaml doc
      newLine.Children.Add(tempTB);
      newLine.Children.Add(tempBttn);
      tempSP.Children.Add(newLine);
      list_of_lines.Children.Add(tempSP);

     


    }
  }

  
  



}


/*
 <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="5*"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
            
                <TextBox x:Name="TextBox1" Padding="0 5"></TextBox>
                <Button x:Name="Button1" Click="Button1_Click" Grid.Column="1" Content="Button" Padding="0 5" Margin="10 0 0 0" />
            </Grid>
   #



      <TextBox x:Name="TextBox1" Padding="0 5"></TextBox>
      <Button x:Name="Button1" Click="Button1_Click" Grid.Column="1" Content="Button" Padding="0 5" Margin="10 0 0 0" />
   */


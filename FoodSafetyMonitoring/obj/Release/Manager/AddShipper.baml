   M S B A M L   `   `   `  ����   	ZRDSystem-    'FoodSafetyMonitoring.Manager.AddShipper   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    $	��510��$	��463��  � WindowStyle$
  None=� � WindowStartupLocation$ CenterScreen=�
������  � 
ResizeMode$ NoResize=�
������  � AllowsTransparency .  � 	Resources  �� MergedDictionaries �� Source��  �� $ Themes/StackPanel.xaml?��� $ Themes/TextBlock.xaml?��� $ Themes/Combobox.xaml?��� $ Themes/TextBox.xaml?��� $" Themes/ImageButtonStyle.xaml?�%    .��� ����  �� ImageSource$ ../res/sampling.png�� � RowDefinitions �� $��45 ��� �� $	��120 �.+�� $��
13,11,11,0q�.+�n� -   $	��440��$��35��
������ 	 J� Opacity$	 0Q�$
��Left=��� $��20��$	��160��$
��Left=�$��16(�$��Center=�
�������货主信息录入�� -   
 ��Name$

 exit��$��29��$��26��$��/res/close.png�� � ToolTip$ 关闭��$��Right=�$	��Top=�� -    �Name$ _grid_detail��$��
13,50,13,0q�$	��Top=�$��Center=�$��1�� �� $��37 ��� $��37 ��� $��37 ��� $��37 ��� $��37 ��� $��37 �.+�� $��0��.+��� $��Center=�.+��� $��40��	代码：�� -    ��Name$	 _id��$	��240�� �� 	MaxLength$ 30��{�.  � $��1��.+��� $��Center=�.+��� $��40��	姓名：�� -   $ _name��$	��240��$ 5��� $��2��.+��� $��Center=�.+��� $��40��	电话：�� -   $ _phone��$	��240��$ 11�� �� IsInputMethodEnabled .  � $��3��.+��� $��Center=�.+��� $��40��县(区)：�� -   $ _region��$	��240��$ 30��� $��4��.+��� $��Center=�.+��� $��40��乡(镇)：�� -   $ _town��$	��240��$ 30��� $��5��.+��� $��Center=�.+��� $��40��村(场)：�� -	   $ _village��$	��240��$ 30��� $��2��$��
13,0,13,50q�.+��� -
   $��btnSave��$	��120��$��32��   SaveImageButtonTemplate#��[  
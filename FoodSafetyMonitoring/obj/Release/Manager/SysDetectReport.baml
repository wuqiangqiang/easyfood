   M S B A M L   `   `   `  ���� D  @ZRDSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:FoodSafetyMonitoring.Manager.UserControls)FoodSafetyMonitoring.Manager.UserControls  2    ,FoodSafetyMonitoring.Manager.SysDetectReport   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    Icontrol7clr-namespace:FoodSafetyMonitoring.Manager.UserControls    ;toolkit-http://schemas.microsoft.com/wpf/2008/toolkit    � 	Resources   �� MergedDictionaries �� Source��  �� $" Themes/ImageButtonStyle.xaml?��� $ Themes/StackPanel.xaml?��� $ Themes/ComboBox.xaml?��� $ Themes/TextBlock.xaml?��� $ Themes/DatePicker.xaml?��� $  Themes/ListViewColumn.xaml?�%    .���   C   =FoodSafetyMonitoring.Manager.UserControls.BackGroundConverter   
_converter  _style_listviewItem �� 
TargetType �� Property �� Value �� 	Converter �� RelativeSource��%  &	        &	      7     �� " ��.���  � �� �� 8    �� *Self+��  � ��
 �   ���  � ��$ Center=���  � ��$ 微软雅黑)���  � ��$ 12(���  � ��$ 35����  � ��$ 0,0,0,0q���  � �� �#e0e0e0 � RowDefinitions �� $	��143 ��� .+�� $��0��$
��auto��$��5,5,5,5q� �� $��37 ��� $
��auto �.+�� $��0��	 � ColumnDefinitions	 �� $	��352 ��� .+�� $��0������ 
 �� ImageSource$
 /res/query_title.png��.+��� $��40,0,0,0q�$��Center=�查询设置� $��1������ $
 /res/title_right.png���� $��1�����#fbfbfc$��1,0,1,1q����#e0e0e0 �� CornerRadius$ 0,0,3,3��._�� $��	0,10,0,10q� �� $��37 ��� $��37 �.+�� $��0��.+��� $��Center=�.+��� $��50��设置日期:+  %Microsoft.Windows.Controls.DatePicker  -     Name$ dtpStartDate��$��Center=�$��26���� 2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve$��10��$��5,0,5,0q�至  -   $ 
dtpEndDate��$��Center=��� $��50��来源产地:�� -    ��Name$ 
_province1��$	��120�� �� Text$	 省��$��0,0,5,0q��� -   $ _city1��$	��120��$ 市(州)��$��0,0,5,0q��� -   $ _region1��$	��120��$ 区县���� $��50��	货主:�� -   $ _source_company1��$	��240��� $��1��.+��� $��Center=�.+��� $��50��检测单位:�� -   $ _detect_station��$	��260��$��Center=��� $��52��
检测师:�� -   $ _detect_person1��$	��120���� -	   $��_query��$	��120��$��32��  SelectImageButtonTemplate#��[ $��	270,0,0,0q��� -
   $��_export��$	��120��$��32��  ExportImageButtonTemplate#��[ $��50,0,0,0q�� $��1��$��5,5,5,5q� �� $��37 ��� .+�� $��0��	 �� $	��352 ��� .+�� $��0������ $
 /res/result_title.png��.+��� .+��� $��40,0,0,0q�$��Center=�查询结果�� -    ��Name$	 _hj��$��20,0,0,0q�$��Center=�$y�Hidden=�合计�� -   $ _title��$��	10,0,10,0q�$��Center=�$��10�����#1956a2$
��Bold"��� -   $	 _sj��$��0,0,0,0q�$��Center=�$y�Hidden=�	条数据� $��1������ $
 /res/title_right.png���� $��1��$��1,0,1,1q����#e0e0e0$ 0,0,3,3�����#fbfbfc._��� $
��Auto=�$��Disabled=�.���  �� $
��auto ��� .+�� $��0�����#f2f7fb	 �� ����  �� ElementName$ Column1�� �� Path$ ActualWidth��� ���� $ Column2��$ ActualWidth��� ���� $ Column3��$ ActualWidth��� ���� $ Column4��$ ActualWidth��� ���� $ Column5��$ ActualWidth��� ���� $ Column6��$ ActualWidth��� ���� $ Column7��$ ActualWidth��� ���� $ Column8��$ ActualWidth��� ���� $ Column9��$ ActualWidth��� ���� $ Column10��$ ActualWidth��� ���� $ Column11��$ ActualWidth��� ���� $ Column12��$ ActualWidth��� ���� $ Column13��$ ActualWidth��� ���� $ Column14��$ ActualWidth�.+��� $��0��$��1,1,1,1q�
���   �._��� $��Center=�$��Center=�.��检测单位�� $��1��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��货主�� $��2��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��数量�� $��3��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��产地�� $��4��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��检疫证号�� $��5��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��	耳标号�� $��6��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��抽检时间�� $��7��$��2��
���   �$��0,1,1,1q�._��  �� $
��Auto ��� $
��Auto �	 �� ���� $ Column8��$ ActualWidth��� ���� $ Column9��$ ActualWidth�.+��� $��2��$��Center=�.��盐酸克伦特罗�� $��1��$��Stretch=�$��0,1,1,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�数量�� $��1��$��1��$��Stretch=�$��0,1,0,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�	阳性数�� $��9��$��2��
���   �$��0,1,1,1q�._��  �� $
��Auto ��� $
��Auto �	 �� ���� $ Column10��$ ActualWidth��� ���� $ Column11��$ ActualWidth�.+��� $��4��$��Center=�.��莱克多巴胺�� $��1��$��Stretch=�$��0,1,1,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�数量�� $��1��$��1��$��Stretch=�$��0,1,0,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�	阳性数�� $��11��$��2��
���   �$��0,1,1,1q�._��  �� $
��Auto ��� $
��Auto �	 �� ���� $ Column12��$ ActualWidth��� ���� $ Column13��$ ActualWidth�.+��� $��4��$��Center=�.��沙丁胺醇�� $��1��$��Stretch=�$��0,1,1,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�数量�� $��1��$��1��$��Stretch=�$��0,1,0,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�	阳性数�� $��13��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��	检测人�� -    ��Name$ livst��$��1��$��Center=�$��Center=�$
��Auto=�$
��Auto=�#��[ ����  �� View �� ! �� ColumnHeaderContainerStyle �� .���  � ��$ 0����  � y�$ Hidden=�.P��� $��检测单位�� �� Width$	 200��-   Column1�� �� DisplayMemberBinding �� *
DeptName+�� $��货主��$	 200��-   Column2�� �� *COMPANYNAME+�� $��数量��$ 80��-   Column3�� �� *objectcount+�� $��产地��$	 170��-   Column4�� �� *
AreaName+�� $��检疫证号 ��$	 150��-   Column5�� �� *CARDID+�� $��	耳标号��$	 200��-   Column6�� �� *objectlabel+�� $��抽检时间��$	 110��-   Column7�� �� *
DETECTDATE+�� $��数量��$ 80��-   Column8�� �� *item_1+�� $��	阳性数��$ 80��-   Column9�� �� *item_1_yang+�� $��数量��$ 80��-   Column10�� �� *item_2+�� $��	阳性数��$ 80��-   Column11�� �� *item_2_yang+�� $��数量��$ 80��-   Column12�� �� *item_3+�� $��	阳性数��$	 100��-   Column13�� �� *item_3_yang+�� $��	检测人��$	 150��-   Column14�� �� *DETECTUSERNAME+
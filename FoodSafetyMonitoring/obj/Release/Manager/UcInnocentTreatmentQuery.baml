   M S B A M L   `   `   `  ���� D  @ZRDSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:FoodSafetyMonitoring.Manager.UserControls)FoodSafetyMonitoring.Manager.UserControls  ;    5FoodSafetyMonitoring.Manager.UcInnocentTreatmentQuery   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    Icontrol7clr-namespace:FoodSafetyMonitoring.Manager.UserControls    ;toolkit-http://schemas.microsoft.com/wpf/2008/toolkit    � 	Resources   �� MergedDictionaries �� Source��  �� $" Themes/ImageButtonStyle.xaml?��� $ Themes/StackPanel.xaml?��� $ Themes/ComboBox.xaml?��� $ Themes/TextBlock.xaml?��� $ Themes/TextBox.xaml?��� $ Themes/DatePicker.xaml?��� $  Themes/ListViewColumn.xaml?�%    .���   E   ?FoodSafetyMonitoring.Manager.UserControls.BtnIsVisibleConverter   _bivC   =FoodSafetyMonitoring.Manager.UserControls.BackGroundConverter  
_converter  _style_listviewItem �� 
TargetType �� Property �� Value �� 	Converter �� RelativeSource��%  &	        &	      &	 
     7      �� " ��.���  � �� �� 8    �� *Self+��  � ��
 �   ���  � ��$ Center=���  � ��$ 微软雅黑)���  � ��$ 12(���  � ��$ 35����  � ��$ 0,0,0,1q���  � �� �#e0e0e0 � RowDefinitions �� $	��143 ��� .+�� $��0��$
��auto��$��5,5,5,5q� �� $��37 ��� $
��auto �.+�� $��0��	 � ColumnDefinitions	 �� $	��352 ��� .+�� $��0������ 
 �� ImageSource$
 /res/query_title.png��.+��� $��40,0,0,0q�$��Center=�查询设置� $��1������ $
 /res/title_right.png���� $��1�����#fbfbfc$��1,0,1,1q����#e0e0e0 �� CornerRadius$ 0,0,3,3��._�� $��	0,10,0,10q� �� $��37 ��� $��37 �.+�� $��0��.+��� $��Center=�.+��� $��50��设置日期:+  %Microsoft.Windows.Controls.DatePicker  -     Name$ dtpStartDate��$��Center=�$��26���� 2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve$��10��$��5,0,5,0q�至  -   $ 
dtpEndDate��$��Center=��� $��50��检测单位:�� -    ��Name$ _detect_station��$	��220��$��Center=��� $��52��
检测师:�� -   $ _detect_person1��$	��100���� $��52��货主姓名:�� -   $ _shipper_name��$	��120�� �� 
IsEditable . � $��1��.+��� $��Center=�.+��� $��50��$"《检疫处理通知单》编号:�� -    ��Name$ _qua_cardid��$	��173��$��Center=��� -   $��_query��$	��120��$��32��  SelectImageButtonTemplate#��[ $��	440,0,0,0q��� -   $��_export��$	��120��$��32��  ExportImageButtonTemplate#��[ $��50,0,0,0q�� $��1��$��5,5,5,5q� �� $��37 ��� .+�� $��0��	 �� $	��352 ��� .+�� $��0������ $
 /res/result_title.png��.+��� .+��� $��40,0,0,0q�$��Center=�查询结果�� -	    ��Name$	 _hj��$��20,0,0,0q�$��Center=�$y�Hidden=�合计�� -
   $ _title��$��	10,0,10,0q�$��Center=�$��10�����#1956a2$
��Bold"��� -   $	 _sj��$��0,0,0,0q�$��Center=�$y�Hidden=�	条数据� $��1������ $
 /res/title_right.png���� $��1��$��1,0,1,1q����#e0e0e0$ 0,0,3,3�����#fbfbfc._��� $
��Auto=�$��Disabled=�.���  �� $
��auto ��� .+�� $��0�����#f2f7fb	 �� ����  �� ElementName$ Column1�� �� Path$ ActualWidth��� ���� $ Column2��$ ActualWidth��� ���� $ Column3��$ ActualWidth��� ���� $ Column4��$ ActualWidth��� ���� $ Column5��$ ActualWidth��� ���� $ Column6��$ ActualWidth��� ���� $ Column7��$ ActualWidth��� ���� $ Column8��$ ActualWidth��� ���� $ Column9��$ ActualWidth��� ���� $ Column10��$ ActualWidth��� ���� $ Column11��$ ActualWidth��� ���� $ Column12��$ ActualWidth��� ���� $ Column13��$ ActualWidth��� ���� $ Column14��$ ActualWidth�.+��� $��0��$��1,1,1,1q�
���   �._��� $��Center=�$��Center=�.��检疫分站�� $��1��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��屠宰场名称�� $��2��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��检疫时间�� $��3��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��货主姓名�� $��4��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��产地�� $��5��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��#!《检疫处理通知单》编号�� $��6��$��2��
���   �$��0,1,1,0q�._��  �� $
��Auto ��� $
��Auto �	 �� ���� $ Column7��$ ActualWidth��� ���� $ Column8��$ ActualWidth�.+��� $��2��$��Center=�.��宰前检查�� $��1��$��Stretch=�$��0,1,1,1q�
���   �._��� $��Center=�$
��Wrap=�$��5q�(&不合格数量(头、只、羽、匹)�� $��1��$��1��$��Stretch=�$��0,1,0,1q�
���   �._��� $��Center=�$
��Wrap=�$��5q�无害化处理方式�� $��8��$��2��
���   �$��0,1,1,0q�._��  �� $
��Auto ��� $
��Auto �	 �� ���� $ Column9��$ ActualWidth��� ���� $ Column10��$ ActualWidth�.+��� $��4��$��Center=�.��同步检疫�� $��1��$��Stretch=�$��0,1,1,1q�
���   �._��� $��Center=�$
��Wrap=�$��5q�(&不合格数量(头、只、羽、匹)�� $��1��$��1��$��Stretch=�$��0,1,0,1q�
���   �._��� $��Center=�$
��Wrap=�$��5q�无害化处理方式�� $��10��$��2��
���   �$��0,1,1,1q�._��  �� $
��Auto ��� $��59 �	 �� ���� $ Column11��$ ActualWidth��� ���� $ Column12��$ ActualWidth�.+��� $��4��$��Center=�.��检疫人员�� $��1��$��Stretch=�$��0,1,1,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�官方兽医姓名�� $��1��$��1��$��Stretch=�$��0,1,0,0q�
���   �._��� $��Center=�$
��Wrap=�$��5q�	协检员�� $��12��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��备注�� $��13��$��0,1,1,1q�
���   �._��� $��Center=�$��Center=�.��删除�� -    ��Name$ livst��$��1��$��Center=�$��Center=�$
��Auto=�$
��Auto=�#��[ ����  �� View �� ! �� ColumnHeaderContainerStyle �� .���  � ��$ 0����  � y�$ Hidden=�.P��� $��检疫分站�� �� Width$	 200��-   Column1�� �� DisplayMemberBinding �� *
deptname+�� $��屠宰场名称��$	 200��-   Column2�� �� *	tzcname+�� $��检疫时间��$	 150��-   Column3�� �� *
createdate+�� $��货主姓名��$	 100��-   Column4�� �� *	sbrname+�� $��产地��$	 200��-   Column5�� �� *area+�� $'��!《检疫处理通知单》编号��$	 150��-   Column6�� �� *
qua_cardid+�� $��不合格数量��$ 80��-   Column7�� �� *no_zq+�� $��无害化处理方式��$ 80��-   Column8�� �� *function_zq+�� $��不合格数量��$ 80��-   Column9�� �� *no_tb+�� $��无害化处理方式��$ 80��-   Column10�� �� *function_tb+�� $��官方兽医��$	 100��-   Column11�� �� *	cusername+�� $��	协检员��$	 100��-   Column12�� �� *helpusername+�� $��备注��$	 140��-   Column13�� �� *bz+�� $ 80��$��删除��-   Column14�� �� CellTemplate �� .R���  ��Name$ _btn_delete��-   $��20�� ! ListDeleteImageButtonTemplate#��[ y��� $ id�# [  
 � Tag �� *id+
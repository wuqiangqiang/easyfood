   M S B A M L   `   `   `  ����   	ZRDSystem-    'FoodSafetyMonitoring.UserControlTemplet   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      8x,http://schemas.microsoft.com/winfx/2006/xaml    
�������  � 	Resources     img �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value �� ScaleX �� ScaleY��%�   &	        �� " ��.���  � ��$ Center=���  �@J�RenderTransformOrigin$ 0.5,0.5&���  �@J�Opacity$	 0.7Q� P�  �@J�IsMouseOver . . ���  � z� �� $	 1.2Q�$	 1.2Q���  �@J�Opacity$ 1Q�.��� 	 � RowDefinitions	 �� $��35 ��� .+�� $��0������ 
 �� ImageSource$
 res/条件栏背head.png�� j� Stretch$
 Fill=� � ColumnDefinitions �� �� .+��� -    ��Name$ _title��$��18(�$
��Left=�$��5q�
�������$��Center=�$��楷体)�$	��600"�$��系统日志管理���� -    ��Name$
 exit��$��1��$��Right=�$��0,0,10,0q�$��25��#��[  $��/res/close.png�� � ToolTip$ 关闭��� -    �Name$
 grid��$��1������ $
 res/信息栏背body.png��$
 Fill=�
   M S B A M L   `   `   `  ����   ChessWPF!    ChessWPF.Pages.MainMenuPage   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� 
Background  �#1A1A1A.��� $��Center=�$��Center=�.+�� .+���    	CardBrush#��[   �� CornerRadius$ 10�� �� Padding$ 40q�$��Center=�$��Center=�$	��500��._��� .+��� $��New Game Setup��$��28(�$
��Bold"�  AccentBrush#��[ $��Center=��� $��Player 1 Name��
�������$��14(��� -    ��Name$ 
Player1Box��  InputBoxStyle#��[ �� $��Opponent��
�������$��14(�$��0,10,0,0q���  �� Orientation$ 
Horizontal=�$��0,5,0,15q�.+�	� -    	�Name$ 
HumanRadio��$��Human��
������� d� 	IsChecked$
 TrueY�$��0,0,20,0q�	� -   $ BotRadio��$��Computer��
��������� $ 
Horizontal=�$��0,5,0,15q�����  �� 
TargetType" ��.���  �� Property � y�	 �� Value$	 	Collapsed=�
 �� Triggers
 ��  �� Binding �� *	IsChecked+ �� ElementName$ BotRadio�� �� Value True.a���  � y�$	 Visible=�.+��� $��Play as:��
�������$��14(�$��Center=�$��0,0,10,0q�	� -   $ 
WhiteRadio��$��White��
�������$
 TrueY�$��0,0,10,0q�	� -   $ 
BlackRadio��$��Black��
��������� $��
Difficulty��
�������$��14(��� -    ��Name$ DifficultyBox��  ComboBoxStyle#��[ {��� *	IsChecked+$ BotRadio��.@��� $
��Easy���� $��Medium���� $
��Hard���� $��Player 2 Name��
�������$��14(��� -   $ 
Player2Box��#��[ {��� *	IsChecked+$ 
HumanRadio���� $��Time Format��
�������$��14(�$��0,10,0,0q��� -   $ TimeBox��#��[ .@��� $��	3 minutes���� $��	5 minutes���� $��
10 minutes���� -	   $��
Start Game��  NavButtonStyle#��[ 
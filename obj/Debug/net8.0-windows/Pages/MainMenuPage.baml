   M S B A M L   `   `   `  ����  ChessWPF!    ChessWPF.Pages.MainMenuPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� 
Background  �#1A1A1A.��5      � $��Center=�6   $��Center=�6(   .+�5   
   � .+�5      ��    	CardBrush#��[  6    �� CornerRadius$ 10��6=    �� Padding$ 40q�6O   $��Center=�5      $��Center=�6.   $	��500��6I   ._�5	      �� .+�5
      �� $��New Game Setup��6    $��28(�66   $
��Bold"�6D     AccentBrush#��[ 5      $��Center=�6F   �� 5      $��Player 1 Name��6    
�������$��14(�6H   �� 5      -   5         ��Name$ 
Player1Box��5        InputBoxStyle#��[ 62   �� 5      $��Opponent��6    
�������$��14(�6C   $��0,10,0,0q�6Q   �� 5       �� Orientation$ 
Horizontal=�6!   $��0,5,0,15q�6:   .+�5      	� -   5         	�Name$ 
HumanRadio��5   &   $��Human��6:   
������� d� 	IsChecked$
 TrueY�6]   $��0,0,20,0q�6n   	� 5      -   5        $ BotRadio��5   &   $��Computer��68   
�������6K   5      �� 5      $ 
Horizontal=�6!   $��0,5,0,15q�6:   ��5      �� 5       �� 
TargetType" ��6$   .�5   "   ��  �� Property � y�	 �� Value$	 	Collapsed=�6?   
 �� Triggers
 5   "   �� 5   &    �� Binding 62   �� *	IsChecked+ �� ElementName$ BotRadio�� �� Value True6f   .a�5   *   ��  � y�$	 Visible=�6G   5   '   5      .+�5      �� $��Play as:��6$   
�������$��14(�6G   $��Center=�6U   $��0,0,10,0q�6p   	� 5       -   5        $ 
WhiteRadio��5    &   $��White��6:   
�������$
 TrueY�6]   $��0,0,10,0q�6n   	� 5!      -   5        $ 
BlackRadio��5!   &   $��Black��6:   
�������6J   5"      �� 5#      $��
Difficulty��6    
�������$��14(�6E   �� 5$      -   5         ��Name$ DifficultyBox��5$        ComboBoxStyle#��[ 66   {�5%      �� *	IsChecked+$ BotRadio��.@�5&      �� $
��Easy��6'   �� 5'      $��Medium��6'   �� 5(      $
��Hard��6'   5)      �� 5+      $��Player 2 Name��6    
�������$��14(�6H   �� 5,      -   5        $ 
Player2Box��5,      #��[ 62   {�5-      �� *	IsChecked+$ 
HumanRadio���� 5.      $��Time Format��6    
�������$��14(�6F   $��0,10,0,0q�6T   �� 5/      -   5        $ TimeBox��5/      #��[ 60   .@�50      �� $��	3 minutes��6'   �� 51      $��	5 minutes��6'   �� 52      $��
10 minutes��6'   53      �� 54      -	   5        $��
Start Game��54        NavButtonStyle#��[ 62   6Z   55      56      57      58      59      
   M S B A M L   `   `   `  ����  ChessWPF#    ChessWPF.Pages.ChessBoardPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� 
Background  �#2E2B25.��5      � $��Center=�6   $��Center=�6(    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $��0,0,0,20q�6X   .+�5      ��    	CardBrush#��[  6    �� CornerRadius$ 5��6=    �� Padding$ 10q�6N   $��5q�6[   ._�5      �� -   5         ��Name$ Player1Timer��5      
�������$��18(�6a   $
��Bold"�6o   ��62   �� *
_whiteTime+6o   5      �� 5      #��[  6   $ 5��6=   $ 10q�6N   $��5q�6[   ._�5      �� -   5        $ Player2Timer��5      
�������$��18(�6b   $
��Bold"�6p   ��62   �� *_blackTimer+6p   5      5      � 5   
   $��1��6   $��20q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      �� -   5         ��Name$ CapturedWhitePanel��5      $��0��65   $��Center=�6E   $��10q�6`   5      �� 5       $��1��6   .+�5!      ��   AccentBrush#��[ 6   $��3q�6D   $ 5��6X   $ 5q�6i   ._�5"      A� -   5         A�Name$ ChessBoardGrid��5"   "   	 A� Rows$	 8��6:   
 A� Columns$
 8��6C   $	��640��5#   "   $	��640��6.   5%      5&      �� 5'      -   5        $
��Undo��5'        NavButtonStyle#��[ 6(   $��Center=�6c   $��0,10,0,0q�6�   5(      �� 5*      -   5        $ CapturedBlackPanel��5*      $��2��65   $��Center=�6E   $��10q�6`   5,      5-      5.      5/      
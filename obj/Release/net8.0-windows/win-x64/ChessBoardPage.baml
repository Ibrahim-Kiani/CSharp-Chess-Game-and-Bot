   M S B A M L   `   `   `  ����   ChessWPF#    ChessWPF.Pages.ChessBoardPage   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� 
Background  �#2E2B25.��� $��Center=�$��Center=� � RowDefinitions �� $
��Auto ��� $��* �.+��� $��0�� �� Orientation$ 
Horizontal=�$��Center=�$��0,0,0,20q�.+���    	CardBrush#��[   �� CornerRadius$ 5�� �� Padding$ 10q�$��5q�._��� -    ��Name$ Player1Timer��
�������$��18(�$
��Bold"����� *
_whiteTime+�� #��[  $ 5��$ 10q�$��5q�._��� -   $ Player2Timer��
�������$��18(�$
��Bold"����� *_blackTimer+� $��1��$��20q� � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ CapturedWhitePanel��$��0��$��Center=�$��10q��� $��1��.+���   AccentBrush#��[ $��3q�$ 5��$ 5q�._�A� -    A�Name$ ChessBoardGrid��	 A� Rows$	 8��
 A� Columns$
 8��$	��640��$	��640���� -   $
��Undo��  NavButtonStyle#��[ $��Center=�$��0,10,0,0q��� -   $ CapturedBlackPanel��$��2��$��Center=�$��10q�
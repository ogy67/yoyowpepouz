-keepattributes SourceFile,LineNumberTable

-keep class com.google.android.gms.** { *; }
-dontwarn com.google.android.gms.**

-keep class com.google.games.** { *; }
-dontwarn com.google.games.**

# Keep all Unity classes
-keep class com.unity3d.** { *; }
-dontwarn com.unity3d.**

# Keep all classes from the Google Play Games plugin
-keep class com.google.android.gms.games.** { *; }
-dontwarn com.google.android.gms.games.**

-keep class com.android.billingclient.** { *; }
-dontwarn com.android.billingclient.**
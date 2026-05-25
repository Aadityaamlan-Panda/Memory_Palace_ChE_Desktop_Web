package com.verstappen.memorypalace.dto;

/**
 * DTO received from the frontend when a user clicks a traffic-light button.
 * score: 2 = easy (+2 strength), 1 = medium (+1), 0 = tough (-2)
 */
public class ReviewDTO {
    private int score; // 0, 1, or 2

    public int getScore() { return score; }
    public void setScore(int score) { this.score = score; }
}

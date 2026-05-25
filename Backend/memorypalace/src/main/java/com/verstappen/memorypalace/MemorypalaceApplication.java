package com.verstappen.memorypalace;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class MemorypalaceApplication {

	public static void main(String[] args) {
		System.out.println("Starting MemoryPalace backend...");
		SpringApplication.run(MemorypalaceApplication.class, args);
		System.out.println("Backend is running.");
	}
}
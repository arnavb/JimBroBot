PRAGMA foreign_keys = ON;

CREATE TABLE
    user (id INT PRIMARY KEY, discord_id TEXT);

CREATE TABLE
    exercise (
        id INT PRIMARY KEY,
        name TEXT NOT NULL,
        type TEXT NOT NULL,
        user_id INT NOT NULL,
        FOREIGN KEY (user_id) REFERENCES user (id)
    );

CREATE TABLE
    exercise_log (id INT PRIMARY KEY);

CREATE TABLE
    all_set_details (
        id INT PRIMARY KEY,
        exercise_log_id INT NOT NULL,
        FOREIGN KEY (exercise_log_id) REFERENCES exercise_log (id)
    );

CREATE TABLE
    set_exercise_details (
        id INT PRIMARY KEY,
        reps INT NOT NULL,
        weight INT NOT NULL,
        is_warmup BOOLEAN NOT NULL,
        all_set_details_id INT NOT NULL,
        FOREIGN KEY (all_set_details_id) REFERENCES all_set_details (id)
    );

CREATE TABLE
    speed_exercise_details (
        id INT PRIMARY KEY,
        duration_sec INT NOT NULL,
        distance_miles REAL NOT NULL,
        exercise_log_id INT NOT NULL,
        FOREIGN KEY (exercise_log_id) REFERENCES exercise_log (id)
    );

CREATE TABLE
    time_exercise_details (
        id INT PRIMARY KEY,
        duration_sec INT NOT NULL,
        exercise_log_id INT NOT NULL,
        FOREIGN KEY (exercise_log_id) REFERENCES exercise_log (id)
    );